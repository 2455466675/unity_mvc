using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MVC
{
    public enum ConditionType
    {
        Equals          = 0,
        NotEquals       = 1,
        Greater         = 2,
        GreaterOrEquals = 3,
        Less            = 4,
        LessOrEquals    = 5,
    }

    public enum Connector
    {
        And = 0,
        Or  = 1,
    }

    public interface IWhere
    {
        bool Execute(DataContainer data);
    }

    [Serializable]
    public class Condition : IWhere
    {
        private static Dictionary<ConditionType, Func<string, string, bool>> actions;
        static Condition()
        {
            actions = new Dictionary<ConditionType, Func<string, string, bool>>();
            actions[ConditionType.Equals] = Equals;
            actions[ConditionType.NotEquals] = NotEquals;
            actions[ConditionType.Greater] = Greater;
            actions[ConditionType.GreaterOrEquals] = GreaterOrEquals;
            actions[ConditionType.Less] = Less;
            actions[ConditionType.LessOrEquals] = LessOrEquals;
        }

        public string key;
        public ConditionType type;
        public string value;

        /// <summary>
        /// ==
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool Equals(string value1, string value2)
        {
            return string.Equals(value1, value2);
        }
        /// <summary>
        /// !=
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool NotEquals(string value1, string value2)
        {
            return !string.Equals(value1, value2);
        }
        /// <summary>
        /// >
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool Greater(string value1, string value2)
        {
            if (string.Equals(value1, value2))
            {
                return false;
            }

            if (float.TryParse(value1, out float result1) && float.TryParse(value2, out float result2))
            {
                return result2 > result1;
            }

            if (int.TryParse(value1, out int result3) && int.TryParse(value2, out int result4))
            {
                return result4 > result3;
            }

            return false;
        }
        /// <summary>
        /// >=
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool GreaterOrEquals(string value1, string value2)
        {
            if (string.Equals(value1, value2))
            {
                return true;
            }

            if (float.TryParse(value1, out float result1) && float.TryParse(value2, out float result2))
            {
                return result2 >= result1;
            }

            if (int.TryParse(value1, out int result3) && int.TryParse(value2, out int result4))
            {
                return result4 >= result3;
            }

            return false;
        }
        /// <summary>
        /// <
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool Less(string value1, string value2)
        {
            if (string.Equals(value1, value2))
            {
                return false;
            }

            if (float.TryParse(value1, out float result1) && float.TryParse(value2, out float result2))
            {
                return result2 < result1;
            }

            if (int.TryParse(value1, out int result3) && int.TryParse(value2, out int result4))
            {
                return result4 < result3;
            }

            return false;
        }
        /// <summary>
        /// <=
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private static bool LessOrEquals(string value1, string value2)
        {
            if (string.Equals(value1, value2))
            {
                return true;
            }

            if (float.TryParse(value1, out float result1) && float.TryParse(value2, out float result2))
            {
                return result2 <= result1;
            }

            if (int.TryParse(value1, out int result3) && int.TryParse(value2, out int result4))
            {
                return result4 <= result3;
            }

            return false;
        }

        public bool Execute(DataContainer data)
        {
            DataBase db = data.FindDataBase(key);
            if (db == null)
            {
                return false;
            }

            return actions[type].Invoke(value, db.StringValue);
        }
    }

    [Serializable]
    public class ConditionGroup : IWhere
    {
        public Connector type;
        public Condition[] conditions;

        public bool Execute(DataContainer data)
        {
            if (conditions == null || conditions.Length == 0)
            {
                return true;
            }

            bool result = type == Connector.And;

            for (int i = 0; i < conditions.Length; i++)
            {
                if (type == Connector.And)
                {
                    result = result && conditions[i].Execute(data);
                }
                else
                {
                    result = result || conditions[i].Execute(data);
                }
            }

            return result;
        }
    }

    [Serializable]
    public class @Where : IWhere
    {
        public ConditionGroup[] groups;

        public bool Execute(DataContainer data)
        {
            if (groups == null || groups.Length == 0)
            {
                return true;
            }

            bool result = true;

            for (int i = 0; i < groups.Length; i++)
            {
                result = result && groups[i].Execute(data);
            }

            return result;
        }
    }

    public enum OrderType
    {
        Asc  = 0, 
        Desc = 1,
    }

    [Serializable]
    public class OrderConditon
    {
        public string key;
        public OrderType type;

        public IOrderedEnumerable<DataContainer> Order(IEnumerable<DataContainer> datas)
        {
            if (type == OrderType.Asc)
            {
                return datas.OrderBy((data) => data.FindDataBase(key));
            }
            else
            {
                return datas.OrderByDescending((data) => data.FindDataBase(key));
            }
        }

        public IOrderedEnumerable<DataContainer> ThenOrder(IOrderedEnumerable<DataContainer> elements)
        {
            if (type == OrderType.Asc)
            {
                return elements.ThenBy((data) => data.FindDataBase(key));
            }
            else
            {
                return elements.ThenByDescending((data) => data.FindDataBase(key));
            }
        }
    }

    [Serializable]
    public class @Order
    {
        public OrderConditon[] conditons;

        public List<DataContainer> OrderExecute(IEnumerable<DataContainer> datas)
        {
            IOrderedEnumerable<DataContainer> elements = null;

            if (conditons == null || conditons.Length == 0)
            {
                return datas.OrderBy(x => 0).ToList();
            }

            for (int i = 0; i < conditons.Length; i++)
            {
                if (i == 0)
                {
                    elements = conditons[i].Order(datas);
                }
                else
                {
                    elements = conditons[i].ThenOrder(elements);
                }
            }

            return elements.ToList();
        }
    }

    /// <summary>
    /// 
    /// </summary>
	public class ListView : View
	{
        [SerializeField]
        private string field;
        [SerializeField]
        private Where @where = new Where();
        [SerializeField]
        private Order @order = new Order();
        private DataCollection collection;
        private List<DataContainer> datas;

        public List<DataContainer> Datas => datas;
        public int Count => datas != null ? datas.Count : 0;

        public override void UpdateViewField()
        {
            if (dataSet == null)
            {
                return;
            }
            if (string.IsNullOrEmpty(field))
            {
                return;
            }

            collection?.Unbind(OnValueChanged);
            collection = dataSet.FindDataCollection(field);
            collection?.Bind(OnValueChanged);
        }

        protected override void OnUpdateView()
        {
            if (collection == null)
            {
                return;
            }

            var items = collection.Where((d) => where.Execute(d));
            datas = order.OrderExecute(items);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(datas.Count);
            stringBuilder.Append(Environment.NewLine);
            for (int i = 0; i < datas.Count ; i++) 
            {
                stringBuilder.Append(datas[i].ToString());
                stringBuilder.Append(Environment.NewLine);
            }

            Debug.Log($"ListView:{stringBuilder}");
        }
    }
}

