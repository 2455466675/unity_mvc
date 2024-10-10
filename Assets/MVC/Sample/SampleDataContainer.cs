using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    /// <summary>
    /// 
    /// </summary>
	public class SampleDataContainer : MonoBehaviour
	{
        public Game game;


        private void Awake()
        {
            game = new Game(DataContainer.Root.CreateContainer("Game"));
        }

        public void Addition()
        {            
            game.package.ChangeCount(game.package.GetDataBase("count").IntValue + 1);
        }

        public void Subtract()
        {
            game.package.ChangeCount(game.package.GetDataBase("count").IntValue - 1);
        }

        public void RandomChangeCount()
        {
            game.package.RandomChangeCount();
        }
    }

    public class Game : DataProxy
    {
        public Package package;

        public Game(DataContainer container) : base(container) 
        {
            SetBaseValue("id", 001);
            SetBaseValue("name", "test_001");
            SetBaseValue("isRole", true);
            package = new Package(CreateContainer("Package"));
        }
    }

    public class Package : DataProxy
    {
        private DataCollection itemList;
        public Package(DataContainer container) : base(container)
        {
            int count = 20;

            SetBaseValue("count", count);
            itemList = CreateCollection("itemList");

            for (int i = 0; i < count; i++)
            {
                var item = itemList.Append();
                item.SetBaseValue("id", i);
                item.SetBaseValue("count", Random.Range(1, 10) * i);
            }
        }

        public void ChangeCount(int count)
        {
            int r = count - GetDataBase("count").IntValue;
            if (r < 0)
            {
                for (int i = 0; i < Mathf.Abs(r); i++)
                {
                    itemList.RemoveAt(itemList.Count - 1);
                }
            }
            else
            {
                for (int i = 0; i < r; i++)
                {
                    var item = itemList.Append();
                    item.SetBaseValue("id", itemList.Count + i + 1);
                    item.SetBaseValue("count", Random.Range(10, 19) * (itemList.Count + i));
                }
            }
            SetBaseValue("count", count);
        }

        public void RandomChangeCount()
        {
            int index = Random.Range(6, itemList.Count);
            DataContainer item = itemList[index];

            int originCount = item.GetDataBase("count").IntValue;
            int count = Random.Range(1, 100);
            item.SetBaseValue("count", count);
            itemList.NotifyChanged();

            Debug.Log($"RandomChangeCount: id : {item.GetDataBase("id").IntValue}, count : {originCount} -> {count}");
        }
    }
}

