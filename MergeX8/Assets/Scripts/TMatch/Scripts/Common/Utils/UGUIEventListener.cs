﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

 namespace TMatch
 {


     public class UGUIEventListener : EventTrigger
     {
         public delegate void PointerDelegate(GameObject go, PointerEventData data);

         public delegate void VoidDelegate(GameObject go);

         public VoidDelegate onClick;
         public PointerDelegate onDown;
         public PointerDelegate onDrag;
         public PointerDelegate onDragBegin;
         public PointerDelegate onDragEnd;
         public PointerDelegate onEnter;
         public PointerDelegate onExit;
         public PointerDelegate onScroll;
         public VoidDelegate onSelect;
         public PointerDelegate onUp;
         public VoidDelegate onUpdateSelect;
         public VoidDelegate onSubmit;
         public PointerDelegate onLongTap;
         public float LongTapTime = 0.5f;
         private Coroutine _longTapCoroutine;
         private bool _isDown;
         private bool _isInDrag;
         private bool _isInLongTap;

         public void SetLongTapTime(float time)
         {
             LongTapTime = time;
         }

         public static UGUIEventListener Get(GameObject go)
         {
             var listener = go.GetComponent<UGUIEventListener>();
             if (listener == null) listener = go.AddComponent<UGUIEventListener>();
             return listener;
         }

         public override void OnPointerClick(PointerEventData eventData)
         {
             if (onClick != null && !_isInDrag && !_isInLongTap)
             {
                 onClick(gameObject);
             }
         }

         public override void OnPointerDown(PointerEventData eventData)
         {
             _isInLongTap = false;
             _isDown = true;
             if (onDown != null) onDown(gameObject, eventData);
             if (onLongTap != null && LongTapTime > 0)
             {
                 if (_longTapCoroutine != null)
                 {
                     StopCoroutine(_longTapCoroutine);
                     _longTapCoroutine = null;
                 }

                 _longTapCoroutine = StartCoroutine(LongTapProcess(LongTapTime, eventData));
             }
         }

         IEnumerator LongTapProcess(float time, PointerEventData eventData)
         {
             while (time > 0 && _isDown)
             {
                 time = time - Time.fixedUnscaledDeltaTime;
                 yield return null;
             }

             if (time <= 0 && onLongTap != null) //触发长按
             {
                 _isInLongTap = true;
                 onLongTap(gameObject, eventData);
             }

             _longTapCoroutine = null;
         }

         public override void OnPointerEnter(PointerEventData eventData)
         {
             if (onEnter != null) onEnter(gameObject, eventData);
         }

         public override void OnPointerExit(PointerEventData eventData)
         {
             if (onExit != null) onExit(gameObject, eventData);
             _isDown = false;
         }


         public override void OnPointerUp(PointerEventData eventData)
         {
             if (_longTapCoroutine != null)
             {
                 StopCoroutine(_longTapCoroutine);
                 _longTapCoroutine = null;
             }

             if (onUp != null) onUp(gameObject, eventData);
             _isDown = false;
         }

         public override void OnSelect(BaseEventData eventData)
         {
             if (onSelect != null) onSelect(gameObject);
         }

         public override void OnUpdateSelected(BaseEventData eventData)
         {
             if (onUpdateSelect != null) onUpdateSelect(gameObject);
         }

         public override void OnDrag(PointerEventData eventData)
         {
             if (onDrag != null) onDrag(gameObject, eventData);
         }

         public override void OnBeginDrag(PointerEventData eventData)
         {
             _isInDrag = true;
             if (onDragBegin != null) onDragBegin(gameObject, eventData);
         }

         public override void OnEndDrag(PointerEventData eventData)
         {
             _isInDrag = false;
             if (onDragEnd != null) onDragEnd(gameObject, eventData);
         }

         public override void OnScroll(PointerEventData eventData)
         {
             if (onScroll != null)
                 onScroll(gameObject, eventData);
         }

         public override void OnSubmit(BaseEventData eventData)
         {
             if (onSubmit != null)
             {
                 onSubmit(gameObject);
             }
         }

         public void OnDestroy()
         {
             onClick = null;
             onDown = null;
             onDrag = null;
             onDragBegin = null;
             onDragEnd = null;
             onEnter = null;
             onExit = null;
             onScroll = null;
             onSelect = null;
             onUp = null;
             onUpdateSelect = null;
             onSubmit = null;
             onLongTap = null;
         }



     }
 }