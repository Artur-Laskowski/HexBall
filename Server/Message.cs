﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    [Serializable]
    public enum MessageAuthor
    {
        Server,
        Client
    }

    [Serializable]
    public enum MessageType
    {
        /// <summary>
        /// ustaw polaczenie
        /// </summary>
        Establish,
        /// <summary>
        /// client przy wysylaniu kolejnego ruchu
        /// </summary>
        Movement,
        /// <summary>
        /// serwer przy wysylaniu elementów canvas 
        /// </summary>
        Canvas,
        /// <summary>
        /// serwer po nadaniu klientowi indeksu gracza
        /// </summary>
        Player,
        /// <summary>
        /// serwer przy golu
        /// </summary>
        Goal
    }

    [Serializable]
    public class Message
    {
        public MessageAuthor author;
        public MessageType type;
        public Object data;
    }
}
