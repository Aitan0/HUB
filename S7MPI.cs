using System;
using System.Runtime.InteropServices;
using System.Collections;
namespace S7Connection
{
	#region �������ⲿ��ϵ�Ľṹ����
	/// <summary>
	/// ����MPI���Ӳ���
	/// </summary>
	public struct ConnectionInfo{
		/// <summary>
		/// ����CPU��MPI��ַ
		/// </summary>
        public byte Addres;
		/// <summary>
		/// ����CPU�Ļ��ܺ�
		/// </summary>
		public byte Rack;
		/// <summary>
		/// ����CPU�Ĳۺ�
		/// </summary>
		public byte Slot;
	}
	#endregion
	
	/// <summary>
	/// ͨ��MPI��ʽ��������S7300/400 PLC����ͨѶ
	/// </summary>
	public abstract class S7MPI
	{
		#region ������̬�⺯��
		/// <summary>
		/// �������ӣ�ͬһ������ֻ�������һ��
		/// </summary>
		/// <param name="nr">���Ӻ�1-4����Ϊ1</param>
		/// <param name="device">�豸���ƣ���Ϊ"s7online"</param>
		/// <param name="adr_table">���Ӳ���,����Ϊ:MPI��ַ��Ϊ0��ʹ�á�CPU�ۺš�CPU���ܺš�MPI��ַΪ0��־�Ų����б����</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int load_tool(byte nr,string device,byte[,] adr_table);
		/// <summary>
		/// �趨����ĳһ������
		/// </summary>
		/// <param name="no">ָ�����Ӻţ���1��ʼ�����ζ�Ӧload_tool�еĲ���adr_table�����ݵ����Ӳ���˳��</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int new_ss(byte no);
		/// <summary>
		/// �Ͽ�����
		/// </summary>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int unload_tool();
		/// <summary>
		/// ��ȡOutputֵ
		/// </summary>
		/// <param name="no">ָ��QB��</param>
		/// <param name="anzahl">ָ���ж��ٸ�QB�ֽ���Ҫ����</param>
		/// <param name="buffer">���ض�����ֵ</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int a_field_read (int no,int anzahl,ref long buffer);
		/// <summary>
		/// ��Outputд��ֵ
		/// </summary>
		/// <param name="no">ָ��QB��</param>
		/// <param name="anzahl">ָ���ж��ٸ�QB�ֽ���Ҫд��</param>
		/// <param name="buffer">ָ��д��ֵ</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int a_field_write (int no,int anzahl,ref long buffer);
		/// <summary>
		/// ��ȡPLC������״̬
		/// </summary>
		/// <param name="buffer">����0����1��0��ʾRun;1��ʾStop����Restart</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int ag_zustand (ref byte buffer);
		/// <summary>
		/// ����ָ����DB���Ƿ����
		/// </summary>
		/// <param name="buffer">����һϵ�п�Ĵ���״̬��=0�����ڣ�!=0����</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")] 
		private extern static int db_buch (ushort[] buffer);
		/// <summary>
		/// ��DB�ж�ȡ����
		/// </summary>
		/// <param name="dbno">ָ��DB���</param>
		/// <param name="dwno">ָ����ȡ����ʼ����ţ�=0��ʾDBW0,=1��ʾDBW2</param>
		/// <param name="anzahl"> ָ����ȡ�Ķ����ж��ٸ���</param>
		/// <param name="buffer">����ֵ</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int db_read(int dbno, int dwno, ref int anzahl, ref long buffer);
		/// <summary>
		/// ��DB��д������
		/// </summary>
		/// <param name="dbno">ָ��DB���</param>
		/// <param name="dwno">ָ��д�����ʼ����ţ�=0��ʾDBW0,=1��ʾDBW2</param>
		/// <param name="anzahl">ָ��д��Ķ����ж��ٸ���</param>
		/// <param name="buffer">д��ֵ</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int db_write(int dbno, int dwno, ref int anzahl, ref long buffer);
		
		/// <summary>
		/// ��ȡInput��ֵ
		/// </summary>
		/// <param name="no">ָ��IB��</param>
		/// <param name="anzahl">ָ���ж��ٸ�IB�ֽ���Ҫ����</param>
		/// <param name="buffer">��ȡ���ص�ֵ</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int e_field_read (int no,int anzahl,ref long buffer);
		/// <summary>
		/// ��ȡMB������λ״ֵ̬
		/// </summary>
		/// <param name="mbno">ָ��M�ֽں�</param>
		/// <param name="bitno">ָ��λ�ţ���ΧΪ0-7</param>
		/// <param name="retwert">����ֵ��>0��ʾ��λΪ1��=0��ʾ��λΪ0</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static int mb_bittest(short mbno,short bitno,ref short retwert);
		/// <summary>
		/// ��λMB������λ״̬
		/// </summary>
		/// <param name="mbno">ָ��M�ֽں�</param>
		/// <param name="bitno">ָ��λ�ŷ�ΧΪ0-7��</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int mb_resetbit (short mbno,short bitno);
		/// <summary>
		/// ��λMB������λ״̬
		/// </summary>
		/// <param name="mbno">ָ��M�ֽں�</param>
		/// <param name="bitno">ָ��λ�ţ���ΧΪ0-7</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int mb_setbit (short mbno,short bitno);
		/// <summary>
		/// ��ȡM�ֽ�ֵ
		/// </summary>
		/// <param name="no">ָ��M�ֽں�</param>
		/// <param name="anzahl">ָ����ȡ���ֽ���</param>
		/// <param name="buffer">��ȡ��ֵ</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int m_field_read (short no,short anzahl,ref long buffer);
		/// <summary>
		/// д��M�ֽ�ֵ
		/// </summary>
		/// <param name="no">ָ��M�ֽں�</param>
		/// <param name="anzahl">ָ��д����ֽ���</param>
		/// <param name="buffer">д���ֵ����ָ�����ֽ��������ɵ�����С��д���ֵ���������ֵ�Զ�����</param>
		/// <returns></returns>
		[DllImport("w95_s7.dll")]
		private extern static  int m_field_write(short no,short anzahl,ref long buffer);
		[DllImport("w95_s7.dll")]
		private extern static  int mix_read (ushort[,] data, ushort[] buffer);
		[DllImport("w95_s7.dll")]
		private extern static  int mix_write (ushort[,] data, ushort[] buffer);
		#endregion
		#region ˽�б�������
         private static Hashtable errHash;
		#endregion
	
		static S7MPI(){
			#region ��ʼ��������Ϣ
			int code=0;
			errHash=new Hashtable(213);
			errHash.Add(code,"ִ�гɹ�");
			code=202;
			errHash.Add(code,"û�п�����Դ");
			code=203;
			errHash.Add(code,"���ô���");
			code=205;
			errHash.Add(code,"�Ƿ�����");
			code=206;
			errHash.Add(code,"û�з���ģ��");
			code=207;
			errHash.Add(code,"����û��װ��");
			code=208;
			errHash.Add(code,"Ӳ��ʧ��");
			code=209;
			errHash.Add(code,"���ʧ��");
			code=210;
			errHash.Add(code,"�ڴ�ʧ��");
			code=215;
			errHash.Add(code,"����Ϣ");
			code=216;
			errHash.Add(code,"�洢ʧ��");
			code=219;
			errHash.Add(code,"�ڲ���ʱ");
			code=225;
			errHash.Add(code,"ͨ����̫��");
			code=226;
			errHash.Add(code,"�ڲ�����");
			code=231;
			errHash.Add(code,"Ӳ������");
			code=233;
			errHash.Add(code,"sin_serv.exeû������");
			code=234;
			errHash.Add(code,"�б���");
			code=240;
			errHash.Add(code,"scp db�ļ�������");
			code=241;
			errHash.Add(code,"û��ȫ�ֵ�dos�洢����");
			code=242;
			errHash.Add(code,"��������г���");
			code=244;
			errHash.Add(code,"�豸������");
			code=245;
			errHash.Add(code,"��ϵͳ����ȷ");
			code=246;
			errHash.Add(code,"δ֪��");
			code=247;
			errHash.Add(code,"����̫С");
			code=248;
			errHash.Add(code,"����̫С");
			code=249;
			errHash.Add(code,"����ȷ��Protocol");
			code=251;
			errHash.Add(code,"���մ���");
			code=252;
			errHash.Add(code,"���֤����");
			code=257;
			errHash.Add(code,"����û������");
			code=266;
			errHash.Add(code,"�ܾ�Ӧ�����/��ʱ����");
			code=268;
			errHash.Add(code,"���ݲ����ڻ򲻿���");
			code=298;
			errHash.Add(code,"ϵͳû���㹻�Ĵ洢����");
			code=302;
			errHash.Add(code,"��������ȷ");
			code=306;
			errHash.Add(code,"DPRAM���޴洢��");
			code=513;
			errHash.Add(code,"ָ���ӿڲ���ȷ");
			code=514;
			errHash.Add(code,"�ӿڳ����������");
			code=515;
			errHash.Add(code,"ϵͳ�ѱ���ʼ��");
			code=516;
			errHash.Add(code,"����Ĳ����б�");
            code=517;
			errHash.Add(code,"����û�г�ʼ��");
			code=518;
			errHash.Add(code,"������ܱ�����");
			code=519;
			errHash.Add(code,"���ݶβ�����");
			code=768;
			errHash.Add(code,"��ʼ������");
			code=769;
			errHash.Add(code,"��ʼ������");
			code=770;
			errHash.Add(code,"��̫С��DW������");
			code=771;
			errHash.Add(code,"�����������");
			code=784;
			errHash.Add(code,"û���ҵ�HW");
			code=785;
			errHash.Add(code,"HW������");
			code=786;
			errHash.Add(code,"�������ò���ȷ");
			code=787;
			errHash.Add(code,"���������ò���ȷ");
			code=788;
			errHash.Add(code,"HSA����������ȷ");
			code=789;
			errHash.Add(code,"MPI��ַ����");
			code=790;
			errHash.Add(code,"�ѷ���HW�豸");
			code=791;
			errHash.Add(code,"�жϲ�����");
			code=792;
			errHash.Add(code,"�жϱ�ռ��");
			code=793;
			errHash.Add(code,"sapû��ռ��");
			code=794;
			errHash.Add(code,"Զ��վû�б�����");
			code=795;
			errHash.Add(code,"�ڲ�����");
			code=796;
			errHash.Add(code,"ϵͳ����");
			code=797;
			errHash.Add(code,"����ߴ����");
			code=800;
			errHash.Add(code,"Ӳ������");
			code=801;
			errHash.Add(code,"DLL��������");
			code=816;
			errHash.Add(code,"�汾��ͻ");
			code=817;
			errHash.Add(code,"�����com����");
			code=818;
			errHash.Add(code,"Ӳ������");
			code=819;
			errHash.Add(code,"com û������");
			code=820;
			errHash.Add(code,"com ������");
			code=821;
			errHash.Add(code,"��������");
			code=822;
			errHash.Add(code,"û������");
			code=823;
			errHash.Add(code,"�������ܾ�");
			code=896;
			errHash.Add(code,"�ڲ�����");
			code=897;
			errHash.Add(code,"Ӳ������");
			code=898;
			errHash.Add(code,"û���ҵ������������豸");
			code=900;
			errHash.Add(code,"û���ҵ������������豸");
			code=1023;
			errHash.Add(code,"ϵͳ����");
			code=2048;
			errHash.Add(code,"toolbox��ռ��");
			code=16385;
			errHash.Add(code,"���Ӳ��ܱ�ʶ��");
			code=16386;
			errHash.Add(code,"����û������");
			code=16387;
			errHash.Add(code,"�������ڱ�����");
			code=16388;
			errHash.Add(code,"���Ӷ���");
			code=32768;
			errHash.Add(code,"�������ڱ�ռ��");
			code=32769;
			errHash.Add(code,"�ڸò���ģʽ�²�������");
			code=33025;
			errHash.Add(code,"Ӳ������");
			code=33027;
			errHash.Add(code,"����ͨ��������");
			code=33208;
			errHash.Add(code,"�����Ĳ���֧��");
			code=33209;
			errHash.Add(code,"��ַ����ȷ");
			code=33030;
			errHash.Add(code,"��֧�ָ���������");
			code=33031;
			errHash.Add(code,"�������Ͳ�����");
			code=33034;
			errHash.Add(code,"���󲻴���");
			code=33552;
			errHash.Add(code,"CPU�ϵĴ洢����������");
			code=33796;
			errHash.Add(code,"���ش���");
			code=834048;
			errHash.Add(code,"����ȷ��PDU�ߴ�");
			code=34562;
			errHash.Add(code,"��ַ����ȷ");
			code=53761;
			errHash.Add(code,"�������﷨����");
			code=53762;
			errHash.Add(code,"�����������﷨����");
			code=53763;
			errHash.Add(code,"���������﷨����");
			code=53764;
			errHash.Add(code,"�ڴ洢������û�п����ӵĿ�");
			code=53765;
			errHash.Add(code,"�����Ѵ���");
			code=53766;
			errHash.Add(code,"�����Ѵ���");
			code=53767;
			errHash.Add(code,"��EPROM�п��Ѵ���");
			code=53769;
			errHash.Add(code,"�鲻����");
			code=53774;
			errHash.Add(code,"û�п��õĿ�");
			code=53776;
			errHash.Add(code,"���̫��");
			code=53825;
			errHash.Add(code,"�����ȼ�����");
			code=54278;
			errHash.Add(code,"��Ϣ������");
			code=61185;
			errHash.Add(code,"����ȷ��ID2");
			code=65531;
			errHash.Add(code,"�绰�����û�з���");
			code=65534;
			errHash.Add(code,"δ֪����");
			code=65535;
			errHash.Add(code,"��ʱ���󣬼��ӿ�");
			code=72;
			errHash.Add(code,"����ʱ����");
			code=17232;
			errHash.Add(code,"û�п�ִ�е�");
			code=17248;
			errHash.Add(code,"��ʱ");
			code=33541;
			errHash.Add(code,"ע��ʱ����");
			code=33542;
			errHash.Add(code,"������");
			code=36863;
			errHash.Add(code,"�ڲ�����");
			code=17665;
			errHash.Add(code,"����ȷ�Ĳ��������ƽ�������߰�װ����");
			code=17666;
			errHash.Add(code,"û�и�������");
			code=17667;
			errHash.Add(code,"���ƽ�������ܲ���");
			code=17668;
			errHash.Add(code,"������ַ���̫��");
			code=17680;
			errHash.Add(code,"adaptor in Modem operation");
			code=17728;
			errHash.Add(code,"�����ѱ�����");
			code=17729;
			errHash.Add(code,"adaptor in Modem operation����û��ʹ��");
			code=17792;
			errHash.Add(code,"��½�û�������");
			code=17793;
			errHash.Add(code,"��½�������");
			code=41478;
			errHash.Add(code,"æ...");
			code=41479;
			errHash.Add(code,"partner not responding");
			code=41490;
			errHash.Add(code,"���Ӳ�����");
			code=41491;
			errHash.Add(code,"û�в���");
			#endregion
		}
		#region �붯̬�⺯�����Ӧ�Ĺ�������
		/// <summary>
		/// �������ӣ�ͬһ������ֻ�������һ��
		/// </summary>
		/// <param name="cnnNo">���Ӻ�1-4</param>
		/// <param name="cnnInfo">ָ�����Ӳ���</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int Open(byte cnnNo,ConnectionInfo[] cnnInfo){
            int err;
			//���ݲ�������ȷ
			if(cnnInfo.Length<=0){
				return -1;
			}
            byte[,] btr=new byte[cnnInfo.Length+1,4];
			//ת�����ӱ�
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
			//���ó�ʼ������
           err=load_tool(cnnNo,"s7online",btr);
		   return err;
		}
		/// <summary>
		/// �ض�����
		/// </summary>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int Close(){
          return unload_tool();
		}
		/// <summary>
		/// �趨����ĳһ������
		/// </summary>
		/// <param name="activeNo">>ָ�����Ӻţ���1��ʼ�����ζ�ӦOpen�еĲ���cnnInfo�����ݵ����Ӳ���˳��</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int SetActive(byte activeNo){
           return new_ss(activeNo);
		}
		/// <summary>
		/// ��ȡָ��������Output�ֽ�ֵ
		/// </summary>
		/// <param name="startNo">ָ����ʼ�ֽں�</param>
		/// <param name="count">ָ����Ҫ��ȡ��Output�ֽ���</param>
		/// <param name="result">���ض�ȡ�Ľ��</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int QBytes_read (int startNo,int count,out long result){
			long val=0;
			int err;
			err=a_field_read(startNo,count,ref val);
			result=val;
			return err;
		}
		/// <summary>
		/// ��ָ��������Output�ֽ�д��ֵ
		/// </summary>
		/// <param name="startNo">ָ����ʼ�ֽں�</param>
		/// <param name="count">ָ����Ҫд���Output�ֽ���</param>
		/// <param name="setValue">�趨ֵ</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int QBytes_write(int startNo,int count,long setValue){
			return a_field_write(startNo,count,ref setValue);

		}
		/// <summary>
		/// ��ȡPLC������״̬
		/// </summary>
		/// <param name="revValue">����0����1��0��ʾRun;1��ʾStop����Restart</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int GetPLCStatus(out byte revValue){
            int err;byte rev=0;
			err=ag_zustand(ref rev);
			revValue=rev;
			return err;
		}
		/// <summary>
		/// ����ָ����DB���Ƿ����
		/// </summary>
		/// <param name="dbNo">ָ����Ҫ���ԵĿ��</param>
		/// <param name="isExist">true��ÿ���ڣ�����Ϊ������</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
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
		///��ȡָ��������DBW����
		/// </summary>
		/// <param name="dbNo">ָ��DB���</param>
		/// <param name="dwNo">ָ����ȡ����ʼ����ţ�=0��ʾDBW0,=1��ʾDBW2������Ϊ*2</param>
		/// <param name="count"> ָ����ȡ�Ķ����ж��ٸ���</param>
		/// <param name="revValue">����ֵ</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int DBWords_read(int dbNo, int dwNo,int count, out long revValue){
			int err;
			long val=0;
			err= db_read(dbNo,dwNo,ref count,ref val);
            revValue=val;
			return err;
	
		}
		/// <summary>
		/// д��ָ��������DBW����
		/// </summary>
		/// <param name="dbNo">ָ��DB���</param>
		/// <param name="dwNo">ָ����ȡ����ʼ����ţ�=0��ʾDBW0,=1��ʾDBW2������Ϊ*2</param>
		/// <param name="count">ָ��д��Ķ����ж��ٸ���</param>
		/// <param name="setValue">ָ���趨ֵ</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int DBWords_write(int dbNo, int dwNo,int count,long setValue){
            return db_write(dbNo,dwNo,ref count,ref setValue);
		}
		/// <summary>
		/// ��ȡָ��������IB����
		/// </summary>
		/// <param name="startNo">ָ����ʼ�ֽں�</param>
		/// <param name="count">ָ����Ҫ��ȡ��Input�ֽ���</param>
		/// <param name="revValue">����ֵ</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int IBytes_read(int startNo,int count,out long revValue){
			long val=0;
			int err;
			err=e_field_read(startNo,count,ref val);
			revValue=val;
			return err;
		}
		/// <summary>
		///  ��ȡMB������λ״ֵ̬
		/// </summary>
		/// <param name="mbNo">ָ��M�ֽں�</param>
		/// <param name="bitNo">ָ��λ�ţ���ΧΪ0-7</param>
		/// <param name="isTrue">����ֵ��true��ʾ��λΪ1��false��ʾ��λΪ0</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
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
		/// ��λMB������λ״̬
		/// </summary>
		/// <param name="mbNo">ָ��M�ֽں�</param>
		/// <param name="bitNo">ָ��λ�ŷ�ΧΪ0-7</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int MBitReset(short mbNo,short bitNo){
			return mb_resetbit(mbNo,bitNo);
		}
		/// <summary>
		/// ��λMB������λ״̬
		/// </summary>
		/// <param name="mbNo">ָ��M�ֽں�</param>
		/// <param name="bitNo">ָ��λ�ŷ�ΧΪ0-7</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int MBitSet(short mbNo,short bitNo){
            return mb_setbit(mbNo,bitNo);
		}
		/// <summary>
		/// ��ȡָ��������MBֵ
		/// </summary>
		/// <param name="mbNo">ָ��M�ֽں�</param>
		/// <param name="count">ָ����ȡ���ֽ���</param>
		/// <param name="revValue">��ȡ��ֵ</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int MBytes_read (short mbNo,short count,out long revValue){
			int err;
			long val=0;
			err= m_field_read(mbNo,count,ref val);
			revValue=val;
			return err;
		}
		/// <summary>
		/// д��ָ��������MBֵ
		/// </summary>
		/// <param name="mbNo">ָ��M�ֽں�</param>
		/// <param name="count">ָ����ȡ���ֽ���</param>
		/// <param name="setValue">�趨ֵ</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int MBytes_write(short mbNo,short count,long setValue){
			return m_field_write(mbNo,count,ref setValue);
		}
		#endregion
		#region ��չ���к���
		/// <summary>
		/// �����ṩ�Ĵ����뷵�ش�����Ϣ
		/// </summary>
		/// <param name="errCode">������</param>
		/// <returns>���ش�����Ϣ</returns>
		public static string GetErrInfo(int errCode){
			object val=errHash[errCode];
			if(val!=null)
               return val.ToString();
			else
				return "û����صĴ�����Ϣ";
		}
		/// <summary>
		/// ��ȡ������λ״̬
		/// </summary>
		/// <param name="data">���͹���������</param>
		/// <param name="bitNo">ת���ɶ����ƺ�ָ���ĵڼ�λ��0��ʾ������������ұߵĵ�һλ,��ΧΪ0-31</param>
		/// <returns>����ָ����λ״̬��true��ʾΪ1��false��ʾΪ0</returns>
		public static bool GetBit(int data,int bitNo){
			int[] bta=new int[1];
			bta[0]=data;
			System.Collections.BitArray myBA =new System.Collections.BitArray(bta);
			return myBA.Get(bitNo);
		}
		/// <summary>
		/// ���ݸ�����32λ������������32λ��ÿһλ��״ֵ̬
		/// </summary>
		/// <param name="data">ָ��32λ����</param>
		/// <returns>���ر�ʾÿһλ״̬��Ϣ������</returns>
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
		/// �Ը��������е�ĳһλ������λ����
		/// </summary>
		/// <param name="souceData">��Ҫ����λ����������</param>
		/// <param name="bitNo">ָ���ڼ�λ��0�������λ�������ұߵĵ�һλ</param>
		public static void SetBit(ref long souceData,int bitNo){
            long bit=(long)(System.Math.Pow(2,bitNo));
            souceData=souceData|bit; 
		}
		/// <summary>
		/// �Ը��������е�����λλ������λ����
		/// </summary>
		/// <param name="souceData">��Ҫ����λ����������</param>
		/// <param name="bitNo">ָ���ڼ�λ�����飬0�������λ�������ұߵĵ�һλ</param>
		public static void SetBit(ref long souceData,int[] bitNo){
            int len=bitNo.Length;
			long bit=0;
			bool tmp;
				for(int i=0;i<len;i++){
					tmp=false;
					for(int j=0;j<i;j++){//������bitNo���Ƿ����ظ���ֵ������������
						if(bitNo[i]==bitNo[j]){
							tmp=true;   //���ظ���ֵ
							break;
						}
					}
					if(tmp==false)  //�������ظ�ֵ
					   bit=bit+(long)(System.Math.Pow(2,bitNo[i]));

				}
			souceData=souceData|bit; 
		}
		/// <summary>
		/// �Ը����������е�ĳһλ���и�λ����
		/// </summary>
		/// <param name="souceData">��Ҫ����λ����������</param>
		/// <param name="bitNo">ָ���ڼ�λ��0�������λ�������ұߵĵ�һλ</param>
		public static void ResetBit(ref long souceData,int bitNo){
			long bit=(long)(System.Math.Pow(2,bitNo));
			long data=souceData;
			data=data&bit;     //��ȡָ��λ��ֵ
			if(data!=0)
				souceData=souceData^bit;

		}
		/// <summary>
		/// �Ը����������еĶ��λ���и�λ����
		/// </summary>
		/// <param name="souceData">��Ҫ����λ����������</param>
		/// <param name="bitNo">ָ���ڼ�λ��0�������λ�������ұߵĵ�һλ</param>
		public static void ResetBit(ref long souceData,int[] bitNo){
			int len=bitNo.Length;
			for(int i=0;i<len;i++){
				long bit=(long)(System.Math.Pow(2,bitNo[i]));
				long data=souceData;
				data=data&bit;     //��ȡָ��λ��ֵ
				if(data!=0)
					souceData=souceData^bit;
			}
		}

		/// <summary>
		/// �򿪶�һ��PLC������
		/// </summary>
		/// <param name="address">ָ��MPI��ַ</param>
		/// <param name="slot">ָ��CPU�ۺ�</param>
		/// <param name="rack">ָ��CPU���ܺ�</param>
		/// <returns>����10���ƴ���ţ�0��ʾû�д���</returns>
		public static int Open(byte address,byte slot,byte rack){
			int err;
			byte[,] btr=new byte[2,4];
			//ת�����ӱ�
			btr[0,0]=address;
			btr[0,1]=0;
			btr[0,2]=slot;
			btr[0,3]=rack;
			btr[1,0]=0;
			btr[1,1]=0;
			btr[1,2]=0;
			btr[1,3]=0;
			//���ó�ʼ������
			err=load_tool(1,"s7online",btr);
			return err;
		}
		#endregion

	}//end class
}
