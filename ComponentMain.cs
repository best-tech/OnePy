using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Enterprise.AddIn;


namespace OnePy
{
#if (ONEPY45)
    [ComVisible(true), Guid("7653664F-920A-48AF-AE16-7662BF963886"), ProgId("AddIn.OnePy45")]
    public partial class OnePy45 : IInitDone, ILanguageExtender
#else
#if (ONEPY35)
    [ComVisible(true), Guid("F0416ABF-AED5-400A-AC7B-13022717D466"), ProgId("AddIn.OnePy35")]
    public partial class OnePy35 : IInitDone, ILanguageExtender
#endif
#endif
    {
#if (ONEPY45)
        const string c_AddinName = "OnePy45";
#else
#if (ONEPY35)
        const string c_AddinName = "OnePy35";
#endif
#endif
        private static int InstancesCount = 0;
        private static AppDomain domain;
        private bool Initialized;

#if (ONEPY45)
        public OnePy45()
#else
#if (ONEPY35)
        public OnePy35()
#endif
#endif
        {
            //domain = AppDomain.CreateDomain("OnePy", AppDomain.CurrentDomain.Evidence);
            Initialized = false;
            Log.PrepareDir();
        }

#if (ONEPY45)
        ~OnePy45()
#else
#if (ONEPY35)
        ~OnePy35()
#endif
#endif
        {
            DoneAll();
            //AppDomain.Unload(domain);
        }

        void DoneAll()
        {
            if (Initialized)
            {
                CleanAll();
                FreePython();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            Initialized = false;
        }

        void InitByNeed()
        {
            if (!Initialized)
            {
                try
                {
                    LoadConfiguration();
                    InitDLR();
                    prAddReferences(RuntimeConfig.additional_asms);
                    Initialized = true;
                }
                catch (Exception e)
                {
                    ProcessError(e);
                    throw;
                }
            }
        }

        #region "IInitDone implementation"

        public void Init([MarshalAs(UnmanagedType.IDispatch)] object pConnection)
        {
            if (InstancesCount < 1)
            {
                V7Data.V7Object = pConnection;
            }
            InstancesCount += 1;
        }

        public void Done()
        {
            InstancesCount -= 1;
            DoneAll();
            if (InstancesCount < 1)
            {
                V7Data.Clean();
            }
        }

        public void GetInfo([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref object[] info)
        {
            info[0] = 2000;
        }

        #endregion

        public void RegisterExtensionAs([MarshalAs(UnmanagedType.BStr)] ref string bstrExtensionName)
        {
            bstrExtensionName = c_AddinName;
        }

        #region "��������"

        enum Props
        {   //�������� �������������� ������� ����� ������� ����������
            //propMessageBoxIcon = 0,  //����������� ��� MessageBox'�
            //propMessageBoxButtons = 1, //������ ��� MessageBox'a
            LastProp = 0
        }

        public void GetNProps(ref int plProps)
        {	//����� 1� �������� ���������� ��������� �� �� �������
            InitByNeed();
            PyGetNProps(ref plProps);
        }

        public void FindProp([MarshalAs(UnmanagedType.BStr)] string bstrPropName, ref int plPropNum)
        {	//����� 1� ���� �������� ������������� �������� �� ��� ���������� �����
            InitByNeed();
            PyFindProp(bstrPropName, ref plPropNum);
        }

        public void GetPropName(int lPropNum, int lPropAlias, [MarshalAs(UnmanagedType.BStr)] ref string pbstrPropName)
        {	//����� 1� (������������) ������ ��� �������� �� ��� ��������������. lPropAlias - ����� ����������
            InitByNeed();
            PyGetPropName(lPropNum, lPropAlias, ref pbstrPropName);
        }

        public void GetPropVal(int lPropNum, [MarshalAs(UnmanagedType.Struct)] ref object pvarPropVal)
        {	//����� 1� ������ �������� �������
            InitByNeed();
            PyGetPropVal(lPropNum, ref pvarPropVal);
        }

        public void SetPropVal(int lPropNum, [MarshalAs(UnmanagedType.Struct)] ref object varPropVal)
        {	//����� 1� �������� �������� �������
            InitByNeed();
            PySetPropVal(lPropNum, ref varPropVal);
        }

        public void IsPropReadable(int lPropNum, ref bool pboolPropRead)
        {	//����� 1� ������, ����� �������� �������� ��� ������
            InitByNeed();
            PyIsPropReadable(lPropNum, ref pboolPropRead); // ��� �������� �������� ��� ������
        }

        public void IsPropWritable(int lPropNum, ref bool pboolPropWrite)
        {	//����� 1� ������, ����� �������� �������� ��� ������
            InitByNeed();
            PyIsPropWritable(lPropNum, ref pboolPropWrite); // ��� �������� �������� ��� ������
        }

        #endregion

        #region "������"

        enum Methods
        {	//�������� �������������� ������� (�������� ��� �������) ����� ������� ����������
            LastMethod = 0,
        }

        public void GetNMethods(ref int plMethods)
        {	//����� 1� �������� ���������� ��������� �� �� �������
            InitByNeed();
            PyGetNMethods(ref plMethods);
        }

        public void FindMethod([MarshalAs(UnmanagedType.BStr)] string bstrMethodName, ref int plMethodNum)
        {	//����� 1� �������� �������� ������������� ������ (��������� ��� �������) �� ����� (��������) ��������� ��� �������
            InitByNeed();
            PyFindMethod(bstrMethodName, ref plMethodNum);
        }

        public void GetMethodName(int lMethodNum, int lMethodAlias, [MarshalAs(UnmanagedType.BStr)] ref string pbstrMethodName)
        {	//����� 1� (������������) �������� ��� ������ �� ��� ��������������. lMethodAlias - ����� ��������.
            InitByNeed();
            PyGetMethodName(lMethodNum, lMethodAlias, ref pbstrMethodName);
        }

        public void GetNParams(int lMethodNum, ref int plParams)
        {	//����� 1� �������� ���������� ���������� � ������ (��������� ��� �������)
            InitByNeed();
            PyGetNParams(lMethodNum, ref plParams);
        }

        public void GetParamDefValue(int lMethodNum, int lParamNum, [MarshalAs(UnmanagedType.Struct)] ref object pvarParamDefValue)
        {	//����� 1� �������� �������� ���������� ��������� ��� ������� �� ���������
            InitByNeed();
            PyGetParamDefValue(lMethodNum, lParamNum, ref pvarParamDefValue); //��� �������� �� ���������
        }

        public void HasRetVal(int lMethodNum, ref bool pboolRetValue)
        {	//����� 1� ������, ���������� �� ����� �������� (�.�. �������� ���������� ��� ��������)
            InitByNeed();
            PyHasRetVal(lMethodNum, ref pboolRetValue);  //��� ������ � ��� ����� ��������� (�.�. ����� ���������� ��������). 
        }

        public void CallAsProc(int lMethodNum, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref object[] pParams)
        {	//����� ������� ���������� ��������� ��� ��������. � �������� � ��� ���.
            InitByNeed();
            PyCallAsProc(lMethodNum, ref pParams);
        }

        public void CallAsFunc(int lMethodNum, [MarshalAs(UnmanagedType.Struct)] ref object pvarRetValue, [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_VARIANT)] ref object[] pParams)
        {	//����� ������� ���������� ��������� ��� �������.
            InitByNeed();
            PyCallAsFunc(lMethodNum, ref pvarRetValue, ref pParams); //������������ �������� ������ ��� 1�			
        }

        #endregion

    }
}
