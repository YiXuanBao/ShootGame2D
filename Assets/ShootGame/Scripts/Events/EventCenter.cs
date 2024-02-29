using System;
using System.Collections.Generic;

//事件类， 工具类，不用太在意实现细节
public class EventCenter
{
    private static Dictionary<MyEventType, Delegate> m_EventTable = new Dictionary<MyEventType, Delegate>();

    //添加监听
    private static void OnListenerAdding(MyEventType MyEventType, Delegate callBack)
    {
        if (!m_EventTable.ContainsKey(MyEventType))
        {
            m_EventTable.Add(MyEventType, null);
        }
        Delegate d = m_EventTable[MyEventType];
        if (d != null && d.GetType() != callBack.GetType())
        {
            throw new Exception(string.Format("尝试为事件{0}添加不同类型的委托，当前事件所对应的委托是{1},要添加的委托类型为{2}", MyEventType, d.GetType(), callBack.GetType()));
        }
    }
    //移除监听
    private static void OnListenerRemoving(MyEventType MyEventType, Delegate callBack)
    {
        if (m_EventTable.ContainsKey(MyEventType))
        {
            Delegate d = m_EventTable[MyEventType];
            if (d == null)
            {
                throw new Exception(string.Format("移除监听错误：事件{0}没有对应的委托", MyEventType));
            }
            else if (d.GetType() != callBack.GetType())
            {
                throw new Exception(string.Format("移除监听错误：尝试为事件{0}移除不同类型的委托，当前委托类型为{1},要移除的类型为{2}", MyEventType, d.GetType(), callBack.GetType()));
            }
        }
        else
        {
            throw new Exception(string.Format("移除监听错误：没有事件码{0}", MyEventType));
        }
    }
    //移除监听后，刷新监听事件列表
    public static void OnListenerRemoeved(MyEventType MyEventType)
    {
        if (m_EventTable[MyEventType] == null)
        {
            m_EventTable.Remove(MyEventType);
        }
    }

    //六种参数数量的添加监听方法
    //no parameters没有参数
    public static void AddListener(MyEventType MyEventType, CallBack callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack)m_EventTable[MyEventType] + callBack;
    }
    //Single parametener一个参数
    public static void AddListener<T>(MyEventType MyEventType, CallBack<T> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T>)m_EventTable[MyEventType] + callBack;
    }
    //two parametener两个参数
    public static void AddListener<T, X>(MyEventType MyEventType, CallBack<T, X> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X>)m_EventTable[MyEventType] + callBack;
    }
    //three parametener三个参数
    public static void AddListener<T, X, Y>(MyEventType MyEventType, CallBack<T, X, Y> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y>)m_EventTable[MyEventType] + callBack;
    }
    //four parametener四个参数
    public static void AddListener<T, X, Y, Z>(MyEventType MyEventType, CallBack<T, X, Y, Z> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y, Z>)m_EventTable[MyEventType] + callBack;
    }
    //five parametener五个参数
    public static void AddListener<T, X, Y, Z, W>(MyEventType MyEventType, CallBack<T, X, Y, Z, W> callBack)
    {
        OnListenerAdding(MyEventType, callBack);
        m_EventTable[MyEventType] = (CallBack<T, X, Y, Z, W>)m_EventTable[MyEventType] + callBack;
    }

    //六种参数数量情况下的事件移除方法
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

    //六种参数数量情况下的广播方法
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", MyEventType));
            }
        }
    }
    //Single parameters 广播事件
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", MyEventType));
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", MyEventType));
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", MyEventType));
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", MyEventType));
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
                throw new Exception(string.Format("广播事件错误：事件{0}对应委托具有不同的类型", MyEventType));
            }
        }
    }
}
