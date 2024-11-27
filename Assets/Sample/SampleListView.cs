using MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 
    /// </summary>
	public class SampleListView : ListViewHelper
    {
        public ListItem prefab;
        public RectTransform content;

        protected override void OnUpdatedView()
        {
            Debug.Log($"ListView-OnUpdatedView:{view.Count}");

            int count = view.Count;
            int childCount = content.childCount;
            int r = count - childCount;
            
            if ( r >= 0 )
            {
                for (int i = 0; i < r; i++)
                {
                    Instantiate(prefab, content);                   
                }

                for (int i = 0; i < content.childCount; i++)
                {
                    Transform child = content.GetChild(i);
                    ListItem item = child.GetComponent<ListItem>();
                    item.SetDatum(view.Datas[i]);
                    child.gameObject.SetActive(true);
                }
            }
            else
            {
                for ( int i = 0; i < childCount; i++ )
                {                    
                    Transform child = content.GetChild(i);
                    if (i >= view.Count)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        ListItem item = child.GetComponent<ListItem>();
                        item.SetDatum(view.Datas[i]);
                        child.gameObject.SetActive(true);
                    }

                }
            }
        }
    }
}

