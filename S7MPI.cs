using System;
using System.Runtime.InteropServices;
using System.Collections;
namespace S7Connection
{
	#region 定义与外部联系的结构变量
	/// <summary>
	/// 定义MPI链接参数
	/// </summary>
	public struct ConnectionInfo{
		/// <summary>
		/// 定义CPU的MPI地址
		/// </summary>
        public byte Addres;
		/// <summary>
		/// 定义CPU的机架号
		/// </summary>
		public byte Rack;
		/// <summary>
		/// 定义CPU的槽号
		/// </summary>
		public byte Slot;
	}
	#endregion
	
	/// <summary>
	/// 通过MPI方式与西门子S7300/400 PLC进行通讯
	/// </summary>
	public abstract class S7MPI
	{
		#region 声明动态库函数
		/// <summary>
		/// 建立连接，同一个连接只容许调用一次
		/// </summary>
		/// <param name="nr">连接号1-4，设为1</param>
		/// <param name="device">设备名称，设为"s7online"</param>
		/// <param name="adr_table">连接参数,依次为:MPI地址、为0不使用、CPU槽号、CPU机架号。MPI地址为0标志着参数列表结束</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int load_tool(byte nr,string device,byte[,] adr_table);
		/// <summary>
		/// 设定激活某一个连接
		/// </summary>
		/// <param name="no">指定连接号，从1开始，依次对应load_tool中的参数adr_table所传递的连接参数顺序</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int new_ss(byte no);
		/// <summary>
		/// 断开连接
		/// </summary>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int unload_tool();
		/// <summary>
		/// 读取Output值
		/// </summary>
		/// <param name="no">指定QB号</param>
		/// <param name="anzahl">指定有多少个QB字节需要读出</param>
		/// <param name="buffer">返回读出的值</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int a_field_read (int no,int anzahl,ref long buffer);
		/// <summary>
		/// 向Output写入值
		/// </summary>
		/// <param name="no">指定QB号</param>
		/// <param name="anzahl">指定有多少个QB字节需要写入</param>
		/// <param name="buffer">指定写入值</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int a_field_write (int no,int anzahl,ref long buffer);
		/// <summary>
		/// 获取PLC的运行状态
		/// </summary>
		/// <param name="buffer">返回0或者1，0表示Run;1表示Stop或者Restart</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int ag_zustand (ref byte buffer);
		/// <summary>
		/// 测试指定的DB块是否存在
		/// </summary>
		/// <param name="buffer">返回一系列块的存在状态，=0不存在，!=0存在</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int db_buch (ushort[] buffer);
		/// <summary>
		/// 从DB中读取数据
		/// </summary>
		/// <param name="dbno">指定DB块号</param>
		/// <param name="dwno">指定读取的起始字序号，=0表示DBW0,=1表示DBW2</param>
		/// <param name="anzahl"> 指定读取的对象有多少个字</param>
		/// <param name="buffer">返回值</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int db_read(int dbno, int dwno, ref int anzahl, ref long buffer);
		/// <summary>
		/// 向DB中写入数据
		/// </summary>
		/// <param name="dbno">指定DB块号</param>
		/// <param name="dwno">指定写入的起始字序号，=0表示DBW0,=1表示DBW2</param>
		/// <param name="anzahl">指定写入的对象有多少个字</param>
		/// <param name="buffer">写入值</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int db_write(int dbno, int dwno, ref int anzahl, ref long buffer);
		
		/// <summary>
		/// 读取Input的值
		/// </summary>
		/// <param name="no">指定IB号</param>
		/// <param name="anzahl">指定有多少个IB字节需要读出</param>
		/// <param name="buffer">读取返回的值</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int e_field_read (int no,int anzahl,ref long buffer);
		/// <summary>
		/// 获取MB变量的位状态值
		/// </summary>
		/// <param name="mbno">指定M字节号</param>
		/// <param name="bitno">指定位号，范围为0-7</param>
		/// <param name="retwert">返回值，>0表示该位为1，=0表示该位为0</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int mb_bittest(short mbno,short bitno,ref short retwert);
		/// <summary>
		/// 复位MB变量的位状态
		/// </summary>
		/// <param name="mbno">指定M字节号</param>
		/// <param name="bitno">指定位号范围为0-7，</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int mb_resetbit (short mbno,short bitno);
		/// <summary>
		/// 置位MB变量的位状态
		/// </summary>
		/// <param name="mbno">指定M字节号</param>
		/// <param name="bitno">指定位号，范围为0-7</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int mb_setbit (short mbno,short bitno);
		/// <summary>
		/// 读取M字节值
		/// </summary>
		/// <param name="no">指定M字节号</param>
		/// <param name="anzahl">指定读取的字节数</param>
		/// <param name="buffer">读取的值</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int m_field_read (short no,short anzahl,ref long buffer);
		/// <summary>
		/// 写入M字节值
		/// </summary>
		/// <param name="no">指定M字节号</param>
		/// <param name="anzahl">指定写入的字节数</param>
		/// <param name="buffer">写入的值，若指定的字节数可容纳的数据小于写入的值，则溢出的值自动舍弃</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int m_field_write(short no,short anzahl,ref long buffer);
		[DllImport("w95_s7.dll")]
		private extern static  int mix_read (ushort[,] data, ushort[] buffer);
		[DllImport("w95_s7.dll")]
		private extern static  int mix_write (ushort[,] data, ushort[] buffer);
		#endregion
		#region 私有变量声明
         private static Hashtable errHash;
		#endregion
	
		static S7MPI(){
			#region 初始化故障信息
			int code=0;
			errHash=new Hashtable(213);
			errHash.Add(code,"执行成功");
			code=202;
			errHash.Add(code,"没有可用资源");
			code=203;
			errHash.Add(code,"配置错误");
			code=205;
			errHash.Add(code,"非法调用");
			code=206;
			errHash.Add(code,"没有发现模块");
			code=207;
			errHash.Add(code,"驱动没有装载");
			code=208;
			errHash.Add(code,"硬件失败");
			code=209;
			errHash.Add(code,"软件失败");
			code=210;
			errHash.Add(code,"内存失败");
			code=215;
			errHash.Add(code,"无信息");
			code=216;
			errHash.Add(code,"存储失败");
			code=219;
			errHash.Add(code,"内部超时");
			code=225;
			errHash.Add(code,"通道打开太多");
			code=226;
			errHash.Add(code,"内部错误");
			code=231;
			errHash.Add(code,"硬件故障");
			code=233;
			errHash.Add(code,"sin_serv.exe没有启动");
			code=234;
			errHash.Add(code,"有保护");
			code=240;
			errHash.Add(code,"scp db文件不存在");
			code=241;
			errHash.Add(code,"没有全局的dos存储可用");
			code=242;
			errHash.Add(code,"传输过程中出错");
			code=244;
			errHash.Add(code,"设备不存在");
			code=245;
			errHash.Add(code,"子系统不正确");
			code=246;
			errHash.Add(code,"未知码");
			code=247;
			errHash.Add(code,"缓存太小");
			code=248;
			errHash.Add(code,"缓存太小");
			code=249;
			errHash.Add(code,"不正确的Protocol");
			code=251;
			errHash.Add(code,"接收错误");
			code=252;
			errHash.Add(code,"许可证错误");
			code=257;
			errHash.Add(code,"连接没有设置");
			code=266;
			errHash.Add(code,"拒绝应答接收/超时错误");
			code=268;
			errHash.Add(code,"数据不存在或不可用");
			code=298;
			errHash.Add(code,"系统没有足够的存储可用");
			code=302;
			errHash.Add(code,"参数不正确");
			code=306;
			errHash.Add(code,"DPRAM中无存储区");
			code=513;
			errHash.Add(code,"指定接口不正确");
			code=514;
			errHash.Add(code,"接口超过最大数量");
			code=515;
			errHash.Add(code,"系统已被初始化");
			code=516;
			errHash.Add(code,"错误的参数列表");
            code=517;
			errHash.Add(code,"连接没有初始化");
			code=518;
			errHash.Add(code,"句柄不能被设置");
			code=519;
			errHash.Add(code,"数据段不可用");
			code=768;
			errHash.Add(code,"初始化错误");
			code=769;
			errHash.Add(code,"初始化错误");
			code=770;
			errHash.Add(code,"块太小，DW不存在");
			code=771;
			errHash.Add(code,"超过块的数量");
			code=784;
			errHash.Add(code,"没有找到HW");
			code=785;
			errHash.Add(code,"HW有问题");
			code=786;
			errHash.Add(code,"参数配置不正确");
			code=787;
			errHash.Add(code,"波特率配置不正确");
			code=788;
			errHash.Add(code,"HSA参数化不正确");
			code=789;
			errHash.Add(code,"MPI地址错误");
			code=790;
			errHash.Add(code,"已分配HW设备");
			code=791;
			errHash.Add(code,"中断不可用");
			code=792;
			errHash.Add(code,"中断被占用");
			code=793;
			errHash.Add(code,"sap没有占用");
			code=794;
			errHash.Add(code,"远程站没有被发现");
			code=795;
			errHash.Add(code,"内部错误");
			code=796;
			errHash.Add(code,"系统错误");
			code=797;
			errHash.Add(code,"缓存尺寸错误");
			code=800;
			errHash.Add(code,"硬件故障");
			code=801;
			errHash.Add(code,"DLL函数错误");
			code=816;
			errHash.Add(code,"版本冲突");
			code=817;
			errHash.Add(code,"错误的com配置");
			code=818;
			errHash.Add(code,"硬件故障");
			code=819;
			errHash.Add(code,"com 没有配置");
			code=820;
			errHash.Add(code,"com 不可用");
			code=821;
			errHash.Add(code,"串行驱动");
			code=822;
			errHash.Add(code,"没有连接");
			code=823;
			errHash.Add(code,"工作被拒绝");
			code=896;
			errHash.Add(code,"内部错误");
			code=897;
			errHash.Add(code,"硬件故障");
			code=898;
			errHash.Add(code,"没有找到驱动器或者设备");
			code=900;
			errHash.Add(code,"没有找到驱动器或者设备");
			code=1023;
			errHash.Add(code,"系统故障");
			code=2048;
			errHash.Add(code,"toolbox被占用");
			code=16385;
			errHash.Add(code,"连接不能被识别");
			code=16386;
			errHash.Add(code,"连接没有设置");
			code=16387;
			errHash.Add(code,"连接正在被设置");
			code=16388;
			errHash.Add(code,"连接断线");
			code=32768;
			errHash.Add(code,"函数正在被占用");
			code=32769;
			errHash.Add(code,"在该操作模式下不被容许");
			code=33025;
			errHash.Add(code,"硬件故障");
			code=33027;
			errHash.Add(code,"对象通道不容许");
			code=33208;
			errHash.Add(code,"上下文不被支持");
			code=33209;
			errHash.Add(code,"地址不正确");
			code=33030;
			errHash.Add(code,"不支持该数据类型");
			code=33031;
			errHash.Add(code,"数据类型不兼容");
			code=33034;
			errHash.Add(code,"对象不存在");
			code=33552;
			errHash.Add(code,"CPU上的存储库容量不足");
			code=33796;
			errHash.Add(code,"严重错误");
			code=834048;
			errHash.Add(code,"不正确的PDU尺寸");
			code=34562;
			errHash.Add(code,"地址不正确");
			code=53761;
			errHash.Add(code,"块名有语法错误");
			code=53762;
			errHash.Add(code,"函数参数有语法错误");
			code=53763;
			errHash.Add(code,"块类型有语法错误");
			code=53764;
			errHash.Add(code,"在存储介质中没有可连接的块");
			code=53765;
			errHash.Add(code,"对象已存在");
			code=53766;
			errHash.Add(code,"对象已存在");
			code=53767;
			errHash.Add(code,"在EPROM中块已存在");
			code=53769;
			errHash.Add(code,"块不存在");
			code=53774;
			errHash.Add(code,"没有可用的块");
			code=53776;
			errHash.Add(code,"块号太大");
			code=53825;
			errHash.Add(code,"操作等级不够");
			code=54278;
			errHash.Add(code,"信息不可用");
			code=61185;
			errHash.Add(code,"不正确的ID2");
			code=65531;
			errHash.Add(code,"电话服务库没有发现");
			code=65534;
			errHash.Add(code,"未知错误");
			code=65535;
			errHash.Add(code,"超时错误，检查接口");
			code=72;
			errHash.Add(code,"连接时出错");
			code=17232;
			errHash.Add(code,"没有可执行的");
			code=17248;
			errHash.Add(code,"超时");
			code=33541;
			errHash.Add(code,"注册时出错");
			code=33542;
			errHash.Add(code,"适配器");
			code=36863;
			errHash.Add(code,"内部故障");
			code=17665;
			errHash.Add(code,"不正确的参数，调制解调器或者安装错误");
			code=17666;
			errHash.Add(code,"没有更多的入口");
			code=17667;
			errHash.Add(code,"调制解调器功能不够");
			code=17668;
			errHash.Add(code,"传输的字符串太长");
			code=17680;
			errHash.Add(code,"adaptor in Modem operation");
			code=17728;
			errHash.Add(code,"报警已被分配");
			code=17729;
			errHash.Add(code,"adaptor in Modem operation报警没有使用");
			code=17792;
			errHash.Add(code,"登陆用户名错误");
			code=17793;
			errHash.Add(code,"登陆密码错误");
			code=41478;
			errHash.Add(code,"忙...");
			code=41479;
			errHash.Add(code,"partner not responding");
			code=41490;
			errHash.Add(code,"连接不可用");
			code=41491;
			errHash.Add(code,"没有拨号");
			#endregion
		}
		#region 与动态库函数相对应的公开函数
		/// <summary>
		/// 建立连接，同一个连接只容许调用一次
		/// </summary>
		/// <param name="cnnNo">连接号1-4</param>
		/// <param name="cnnInfo">指定链接参数</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int Open(byte cnnNo,ConnectionInfo[] cnnInfo){
            int err;
			//传递参数不正确
			if(cnnInfo.Length<=0){
				return -1;
			}
            byte[,] btr=new byte[cnnInfo.Length+1,4];
			//转换链接表
			for(int i=0;i<cnnInfo.Length;i++){
               btr[i,0]=cnnInfo[i].Addres;
               btr[i,1]=0;
			   btr[i,2]=cnnInfo[i].Slot;
			   btr[i,3]=cnnInfo[i].Rack;
			}
			btr[cnnInfo.Length,0]=0;
			btr[cnnInfo.Length,1]=0;
			btr[cnnInfo.Length,2]=0;
			btr[cnnInfo.Length,3]=0;
			//调用初始化函数
           err=load_tool(cnnNo,"s7online",btr);
		   return err;
		}
		/// <summary>
		/// 关断链接
		/// </summary>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int Close(){
          return unload_tool();
		}
		/// <summary>
		/// 设定激活某一个连接
		/// </summary>
		/// <param name="activeNo">>指定连接号，从1开始，依次对应Open中的参数cnnInfo所传递的连接参数顺序</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int SetActive(byte activeNo){
           return new_ss(activeNo);
		}
		/// <summary>
		/// 读取指定数量的Output字节值
		/// </summary>
		/// <param name="startNo">指定起始字节号</param>
		/// <param name="count">指定需要读取的Output字节数</param>
		/// <param name="result">返回读取的结果</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int QBytes_read (int startNo,int count,out long result){
			long val=0;
			int err;
			err=a_field_read(startNo,count,ref val);
			result=val;
			return err;
		}
		/// <summary>
		/// 向指定数量的Output字节写入值
		/// </summary>
		/// <param name="startNo">指定起始字节号</param>
		/// <param name="count">指定需要写入的Output字节数</param>
		/// <param name="setValue">设定值</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int QBytes_write(int startNo,int count,long setValue){
			return a_field_write(startNo,count,ref setValue);

		}
		/// <summary>
		/// 获取PLC的运行状态
		/// </summary>
		/// <param name="revValue">返回0或者1，0表示Run;1表示Stop或者Restart</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int GetPLCStatus(out byte revValue){
            int err;byte rev=0;
			err=ag_zustand(ref rev);
			revValue=rev;
			return err;
		}
		/// <summary>
		/// 测试指定的DB块是否存在
		/// </summary>
		/// <param name="dbNo">指定需要测试的块号</param>
		/// <param name="isExist">true表该块存在，否则为不存在</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static  int dbExist(int dbNo,out bool isExist){
			int err;
			ushort[] bt=new ushort[500];
			err=db_buch(bt);
			isExist=false;
			if(err==0)
				if(bt[dbNo]!=0)
					isExist=true;
			    else
					isExist=false;
			return err;
		}
		/// <summary>
		///读取指定数量的DBW数据
		/// </summary>
		/// <param name="dbNo">指定DB块号</param>
		/// <param name="dwNo">指定读取的起始字序号，=0表示DBW0,=1表示DBW2，规律为*2</param>
		/// <param name="count"> 指定读取的对象有多少个字</param>
		/// <param name="revValue">返回值</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int DBWords_read(int dbNo, int dwNo,int count, out long revValue){
			int err;
			long val=0;
			err= db_read(dbNo,dwNo,ref count,ref val);
            revValue=val;
			return err;
	
		}
		/// <summary>
		/// 写入指定数量的DBW数据
		/// </summary>
		/// <param name="dbNo">指定DB块号</param>
		/// <param name="dwNo">指定读取的起始字序号，=0表示DBW0,=1表示DBW2，规律为*2</param>
		/// <param name="count">指定写入的对象有多少个字</param>
		/// <param name="setValue">指定设定值</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int DBWords_write(int dbNo, int dwNo,int count,long setValue){
            return db_write(dbNo,dwNo,ref count,ref setValue);
		}
		/// <summary>
		/// 读取指定数量的IB数据
		/// </summary>
		/// <param name="startNo">指定起始字节号</param>
		/// <param name="count">指定需要读取的Input字节数</param>
		/// <param name="revValue">返回值</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int IBytes_read(int startNo,int count,out long revValue){
			long val=0;
			int err;
			err=e_field_read(startNo,count,ref val);
			revValue=val;
			return err;
		}
		/// <summary>
		///  获取MB变量的位状态值
		/// </summary>
		/// <param name="mbNo">指定M字节号</param>
		/// <param name="bitNo">指定位号，范围为0-7</param>
		/// <param name="isTrue">返回值，true表示该位为1，false表示该位为0</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int MBitStatusGet(short mbNo,short bitNo,out bool isTrue){
			int err;
			short val=0;
			err=mb_bittest(mbNo,bitNo,ref val);
			isTrue=false;
			if(err==0)
				if(val>0)
					isTrue=true;
				else
					isTrue=false;
			return err;
		}
		/// <summary>
		/// 复位MB变量的位状态
		/// </summary>
		/// <param name="mbNo">指定M字节号</param>
		/// <param name="bitNo">指定位号范围为0-7</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int MBitReset(short mbNo,short bitNo){
			return mb_resetbit(mbNo,bitNo);
		}
		/// <summary>
		/// 置位MB变量的位状态
		/// </summary>
		/// <param name="mbNo">指定M字节号</param>
		/// <param name="bitNo">指定位号范围为0-7</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int MBitSet(short mbNo,short bitNo){
            return mb_setbit(mbNo,bitNo);
		}
		/// <summary>
		/// 读取指定数量的MB值
		/// </summary>
		/// <param name="mbNo">指定M字节号</param>
		/// <param name="count">指定读取的字节数</param>
		/// <param name="revValue">读取的值</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int MBytes_read (short mbNo,short count,out long revValue){
			int err;
			long val=0;
			err= m_field_read(mbNo,count,ref val);
			revValue=val;
			return err;
		}
		/// <summary>
		/// 写入指定数量的MB值
		/// </summary>
		/// <param name="mbNo">指定M字节号</param>
		/// <param name="count">指定读取的字节数</param>
		/// <param name="setValue">设定值</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int MBytes_write(short mbNo,short count,long setValue){
			return m_field_write(mbNo,count,ref setValue);
		}
		#endregion
		#region 扩展公有函数
		/// <summary>
		/// 根据提供的错误码返回错误信息
		/// </summary>
		/// <param name="errCode">错误码</param>
		/// <returns>返回错误信息</returns>
		public static string GetErrInfo(int errCode){
			object val=errHash[errCode];
			if(val!=null)
               return val.ToString();
			else
				return "没有相关的错误信息";
		}
		/// <summary>
		/// 获取整数的位状态
		/// </summary>
		/// <param name="data">传送过来的整数</param>
		/// <param name="bitNo">转换成二进制后，指定的第几位，0表示由右至左的最右边的第一位,范围为0-31</param>
		/// <returns>返回指定的位状态，true表示为1，false表示为0</returns>
		public static bool GetBit(int data,int bitNo){
			int[] bta=new int[1];
			bta[0]=data;
			System.Collections.BitArray myBA =new System.Collections.BitArray(bta);
			return myBA.Get(bitNo);
		}
		/// <summary>
		/// 根据给定的32位整数，返回这32位中每一位的状态值
		/// </summary>
		/// <param name="data">指定32位整数</param>
		/// <returns>返回表示每一位状态信息的数组</returns>
		public static bool[] GetBit(int data){
			bool[] stau=new bool[32];
			int[] bta=new int[1];
			bta[0]=data;
			System.Collections.BitArray myBA =new System.Collections.BitArray(bta);
			for(int i=0;i<32;i++){
               stau[i]=myBA.Get(i);
			}
			return stau;
		}
		/// <summary>
		/// 对给定数据中的某一位进行置位操作
		/// </summary>
		/// <param name="souceData">需要进行位操作的数据</param>
		/// <param name="bitNo">指定第几位，0代表最低位，即最右边的第一位</param>
		public static void SetBit(ref long souceData,int bitNo){
            long bit=(long)(System.Math.Pow(2,bitNo));
            souceData=souceData|bit; 
		}
		/// <summary>
		/// 对给定数据中的若干位位进行置位操作
		/// </summary>
		/// <param name="souceData">需要进行位操作的数据</param>
		/// <param name="bitNo">指定第几位的数组，0代表最低位，即最右边的第一位</param>
		public static void SetBit(ref long souceData,int[] bitNo){
            int len=bitNo.Length;
			long bit=0;
			bool tmp;
				for(int i=0;i<len;i++){
					tmp=false;
					for(int j=0;j<i;j++){//检测参数bitNo中是否有重复的值，若有责舍弃
						if(bitNo[i]==bitNo[j]){
							tmp=true;   //有重复的值
							break;
						}
					}
					if(tmp==false)  //倘若无重复值
					   bit=bit+(long)(System.Math.Pow(2,bitNo[i]));

				}
			souceData=souceData|bit; 
		}
		/// <summary>
		/// 对给定的数据中的某一位进行复位操作
		/// </summary>
		/// <param name="souceData">需要进行位操作的数据</param>
		/// <param name="bitNo">指定第几位，0代表最低位，即最右边的第一位</param>
		public static void ResetBit(ref long souceData,int bitNo){
			long bit=(long)(System.Math.Pow(2,bitNo));
			long data=souceData;
			data=data&bit;     //获取指定位的值
			if(data!=0)
				souceData=souceData^bit;

		}
		/// <summary>
		/// 对给定的数据中的多个位进行复位操作
		/// </summary>
		/// <param name="souceData">需要进行位操作的数据</param>
		/// <param name="bitNo">指定第几位，0代表最低位，即最右边的第一位</param>
		public static void ResetBit(ref long souceData,int[] bitNo){
			int len=bitNo.Length;
			for(int i=0;i<len;i++){
				long bit=(long)(System.Math.Pow(2,bitNo[i]));
				long data=souceData;
				data=data&bit;     //获取指定位的值
				if(data!=0)
					souceData=souceData^bit;
			}
		}

		/// <summary>
		/// 打开对一个PLC的连接
		/// </summary>
		/// <param name="address">指定MPI地址</param>
		/// <param name="slot">指定CPU槽号</param>
		/// <param name="rack">指定CPU机架号</param>
		/// <returns>返回10进制错误号，0表示没有错误</returns>
		public static int Open(byte address,byte slot,byte rack){
			int err;
			byte[,] btr=new byte[2,4];
			//转换链接表
			btr[0,0]=address;
			btr[0,1]=0;
			btr[0,2]=slot;
			btr[0,3]=rack;
			btr[1,0]=0;
			btr[1,1]=0;
			btr[1,2]=0;
			btr[1,3]=0;
			//调用初始化函数
			err=load_tool(1,"s7online",btr);
			return err;
		}
		#endregion

	}//end class
}
