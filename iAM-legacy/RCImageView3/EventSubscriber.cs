using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoadCare3;
using DataObjects;

namespace RCImageView3
{
    class EventSubscriber
    {
        private EventPublisher m_eventNavigate;
        public EventSubscriber(BaseForm form)
        {
            m_eventNavigate = form.m_event;
            //Here (in simple English), we (this class) want to subscribe to this
            //event delegate (EventDelegate) of EventPublisher. So if the EventPublisher
            //raises any event, please send the event to our method this_OnProgress...
            m_eventNavigate.myEvent += new EventPublisher.EventDelegate(this_OnProgress);
        }

        ~EventSubscriber()
        {
            m_eventNavigate.myEvent -= new EventPublisher.EventDelegate(this_OnProgress);
        }

        //static void Main()
        //{
        //    //Create an instance of this class
        //    EventSubscriber theClass = new EventSubscriber();
        //}

        //private void Start()
        //{
        //    //We create an instance of the EventPublisher
        //    m_eventNavigate = new EventPublisher();

        //    //Here (in simple English), we (this class) want to subscribe to this
        //    //event delegate (EventDelegate) of EventPublisher. So if the EventPublisher
        //    //raises any event, please send the event to our method this_OnProgress...
        //    m_eventNavigate.myEvent += new EventPublisher.EventDelegate(this_OnProgress);

        //    //Here is where the event fires...
        //    //Lets say u're making an email notification program...
        //    //So when an email arrives, this (or some other class, in real life), 
        //    //calls the EventPublisher's SendTheEvent method
        //    m_eventNavigate.SendTheEvent();
        //}

        private void this_OnProgress(object sender, EventArgs e)
        {
            NavigationEvent navigationEvent = (NavigationEvent)e;
            ImageViewManager.Navigation.OnNavigationEvent(navigationEvent);

        
        
        }
    }
}
