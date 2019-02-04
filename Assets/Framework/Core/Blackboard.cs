using System.Collections.Generic;

using BlackboardKey = System.Int32;
public class Blackboard
{
    abstract class BlackboardItem
    {
    }

    class BlackboardItem<T> : BlackboardItem
    {
        private T _value;
        public void SetValue(T v)
        {
            _value = v;
        }
        public T GetValue()
        {
            return _value;
        }
    }

    private Dictionary<BlackboardKey, BlackboardItem> items;

    public Blackboard()
    {
        items = new Dictionary<BlackboardKey, BlackboardItem>();
    }

    public void SetValue<T>(BlackboardKey key, T value)
    {
        BlackboardItem item;
        if (!items.TryGetValue(key, out item))
        {
            item = new BlackboardItem<T>();
            items.Add(key, item);
        }

        var ritem = item as BlackboardItem<T>;
        ritem.SetValue(value);
    }

    public T GetValue<T>(BlackboardKey key, T defaultValue = default(T))
    {
        BlackboardItem item;
        if (!items.TryGetValue(key, out item))
        {
            return defaultValue;
        }

        var ritem = item as BlackboardItem<T>;
        return ritem.GetValue();
    }
}