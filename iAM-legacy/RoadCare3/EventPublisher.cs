using System;
using System.Collections.Generic;
using System.Text;

namespace RoadCare3
{
    public class EventPublisher
    {
        //We declare the event itself here
        public event EventDelegate myEvent;

        //Our delegation here (in simple english), is to 
        //publish our event. It sort of says "hey everyone, we have an
        //event here that you can subscribe to"...
        public delegate void EventDelegate(object from, EventArgs args);

        //This is where we raise the event from the EventPublisher class
        public void issueEvent(EventArgs args)
        {
            if (myEvent != null)
            {
                myEvent(this, args);
            }
        }


        //We don't need any logic for the constructor, to keep it simple
        public EventPublisher()
        {
        }

        //This is the event trigger
        //The reason I seperated this is because I wanted to show you 
        //that we can pass EventArgs to issueEvent
        //Usually we will inherit from EventArgs and create our own
        //data structure to send information pertaining to the event
        public void SendTheEvent()
        {
   //         Console.WriteLine("We fire the event here: SendTheEvent()");
            this.issueEvent(new EventArgs());
        }
    }
}
