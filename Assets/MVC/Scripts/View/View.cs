using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVC
{


    /// <summary>
    /// 
    /// </summary>
	public class View : MonoBehaviour
	{
        [SerializeField]
        protected DataSet dataSet;
        private bool isDirty;

        private event Action OnUpdatedViewEvent;

        protected virtual void Start()
        {
            if (dataSet != null)
            {
                dataSet.Register(this);
            }
        }

        protected virtual void OnDestroy()
        {
            if (dataSet != null)
            {
                dataSet.Unregister(this);
                dataSet = null;
            }
            OnUpdatedViewEvent = null;
        }

        private void OnEnable()
        {
            if (!isDirty)
            {
                return;
            }
            Refresh();
            isDirty = false;
        }

        public void Bind(Action handler)
        {
            if (handler == null)
            {
                return;
            }
            OnUpdatedViewEvent += handler;
        }

        public void Unbind(Action handler)
        {
            if (handler == null)
            {
                return;
            }
            OnUpdatedViewEvent -= handler;
        }

        public virtual void UpdateViewField()
        {           
        }

        protected void OnValueChanged()
        {
            if (!gameObject.activeInHierarchy)
            {
                isDirty = true;
            }
            else
            {
                Refresh();
                isDirty = false;
            }
        }

        private void Refresh()
        {
            OnUpdateView();
            OnUpdatedViewEvent?.Invoke();
        }
  
        protected virtual void OnUpdateView()
        {

        }     
    }
}

