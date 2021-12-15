﻿using System;
using System.Reflection;

namespace Common
{
    public enum Services { PrivateMessage, GroupMessage, GlobalMessage, DB, Exception, Login, Create, Patch, SessionHost, SessionManage, Response }
    public enum RabbitMQExchangeTypes { Direct, Fanout, Headers, Topic }

    public enum QueueInteraction { Listener, Broadcaster, Bidirectional }
    public static class Constants
    {
        public static string Hostname { get; } = "localhost";
    }
}
