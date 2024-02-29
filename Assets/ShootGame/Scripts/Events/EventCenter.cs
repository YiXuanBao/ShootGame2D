using System;
using System.Collections.Generic;

//�¼��࣬ �����࣬����̫����ʵ��ϸ��
public class EventCenter
{
    private static Dictionary<MyEventType, Delegate> m_EventTable = new Dictionary<MyEventType, Delegate>();

    //��Ӽ���
    private static void OnListenerAdding(MyEventType MyEventType, Delegate callBack)
    {
        if (!m_EventTable.ContainsKey(MyEventType))
        {
            m_EventTable.Add(MyEventType, null);
        }
        Delegate d = m_EventTable[MyEventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("����Ϊ�¼�{0}��Ӳ�ͬ���͵�ί�У���ǰ�¼�����Ӧ��ί����{1},Ҫ��ӵ�ί������Ϊ{2}", MyEventType, d.GetType(), callBack.GetType()));
        }
    }
    //�Ƴ�����
    private static void OnListenerRemoving(MyEventType MyEventType, Delegate callBack)
    {
        if (m_EventTable.ContainsKey(MyEventType))
        {
            Delegate d = m_EventTable[MyEventType];
            if (d == null)
            {
                throw new Exception(string.Format("�Ƴ����������¼�{0}û�ж�Ӧ��ί��", MyEventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("�Ƴ��������󣺳���Ϊ�¼�{0}�Ƴ���ͬ���͵�ί�У���ǰί������Ϊ{1},Ҫ�Ƴ�������Ϊ{2}", MyEventType, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            throw new Exception(string.Format("�Ƴ���������û���¼���{0}", MyEventType));
        }
    }
    //�Ƴ�������ˢ�¼����¼��б�
    public static void OnListenerRemoeved(MyEventType MyEventType)
    {
        if (m_EventTable[MyEventType] == null)
        {
            m_EventTable.Remove(MyEventType);
        }
    }

    //���ֲ�����������Ӽ�������
    //no parametersû�в���
    public static void AddListener(MyEventType MyEventType, CallBack callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack)m_EventTable[MyEventType] + callBack;
    }
    //Single parametenerһ������
    public static void AddListener<T>(MyEventType MyEventType, CallBack<T> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T>)m_EventTable[MyEventType] + callBack;
    }
    //two parametener��������
    public static void AddListener<T, X>(MyEventType MyEventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X>)m_EventTable[MyEventType] + callBack;
    }
    //three parametener��������
    public static void AddListener<T, X, Y>(MyEventType MyEventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y>)m_EventTable[MyEventType] + callBack;
    }
    //four parametener�ĸ�����
    public static void AddListener<T, X, Y, Z>(MyEventType MyEventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y, Z>)m_EventTable[MyEventType] + callBack;
    }
    //five parametener�������
    public static void AddListener<T, X, Y, Z, W>(MyEventType MyEventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[MyEventType] + callBack;
    }

    //���ֲ�����������µ��¼��Ƴ�����
    //no parameters
    public static void RemoveListener(MyEventType MyEventType, CallBack callBack)
    {
        OnListenerRemoving(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack)m_EventTable[MyEventType] - callBack;
        OnListenerRemoeved(MyEventType);
    }
    //Single parameters
    public static void RemoveListener<T>(MyEventType MyEventType, CallBack<T> callBack)
    {
        OnListenerRemoving(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T>)m_EventTable[MyEventType] - callBack;
        OnListenerRemoeved(MyEventType);
    }
    //two parameters
    public static void RemoveListener<T, X>(MyEventType MyEventType, CallBack<T, X> callBack)
    {
        OnListenerRemoving(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X>)m_EventTable[MyEventType] - callBack;
        OnListenerRemoeved(MyEventType);
    }
    //three parameters
    public static void RemoveListener<T, X, Y>(MyEventType MyEventType, CallBack<T, X, Y> callBack)
    {
        OnListenerRemoving(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y>)m_EventTable[MyEventType] - callBack;
        OnListenerRemoeved(MyEventType);
    }
    //four parameters
    public static void RemoveListener<T, X, Y, Z>(MyEventType MyEventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerRemoving(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y, Z>)m_EventTable[MyEventType] - callBack;
        OnListenerRemoeved(MyEventType);
    }
    //five parameters
    public static void RemoveListener<T, X, Y, Z, W>(MyEventType MyEventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerRemoving(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[MyEventType] - callBack;
        OnListenerRemoeved(MyEventType);
    }

    //���ֲ�����������µĹ㲥����
    //no parameters
    public static void Broadcast(MyEventType MyEventType)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(MyEventType, out d))
        {
            CallBack callBack = d as CallBack;
            if (callBack != null)
            {
                callBack();
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", MyEventType));
            }
        }
    }
    //Single parameters �㲥�¼�
    public static void Broadcast<T>(MyEventType MyEventType, T arg)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(MyEventType, out d))
        {
            CallBack<T> callBack = d as CallBack<T>;
            if (callBack != null)
            {
                callBack(arg);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", MyEventType));
            }
        }
    }
    //two parameters
    public static void Broadcast<T, X>(MyEventType MyEventType, T arg1, X arg2)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(MyEventType, out d))
        {
            CallBack<T, X> callBack = d as CallBack<T, X>;
            if (callBack != null)
            {
                callBack(arg1, arg2);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", MyEventType));
            }
        }
    }
    //three parameters
    public static void Broadcast<T, X, Y>(MyEventType MyEventType, T arg1, X arg2, Y arg3)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(MyEventType, out d))
        {
            CallBack<T, X, Y> callBack = d as CallBack<T, X, Y>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", MyEventType));
            }
        }
    }
    //four parameters
    public static void Broadcast<T, X, Y, Z>(MyEventType MyEventType, T arg1, X arg2, Y arg3, Z arg4)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(MyEventType, out d))
        {
            CallBack<T, X, Y, Z> callBack = d as CallBack<T, X, Y, Z>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", MyEventType));
            }
        }
    }
    //five parameters
    public static void Broadcast<T, X, Y, Z, W>(MyEventType MyEventType, T arg1, X arg2, Y arg3, Z arg4, W arg5)
    {
        Delegate d;
        if (m_EventTable.TryGetValue(MyEventType, out d))
        {
            CallBack<T, X, Y, Z, W> callBack = d as CallBack<T, X, Y, Z, W>;
            if (callBack != null)
            {
                callBack(arg1, arg2, arg3, arg4, arg5);
            }
            else
            {
                throw new Exception(string.Format("�㲥�¼������¼�{0}��Ӧί�о��в�ͬ������", MyEventType));
            }
        }
    }
}
