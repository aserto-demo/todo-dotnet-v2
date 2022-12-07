using Aserto.AspNetCore.Middleware.Options;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using System;

namespace Aserto.TodoApp.Mapping
{
    public sealed class AuthzResourceContext
    {
        private static readonly AuthzResourceContext instance = new AuthzResourceContext();

        static AuthzResourceContext()
        {

        }
        private AuthzResourceContext()
        {

        }

        public Func<string, HttpRequest, Struct> ResourceMapper { get; set; } = AsertoOptionsDefaults.DefaultResourceMapper;

        public static AuthzResourceContext Instance
        {
         get
            {
                return instance;
            }
        }
    }
}
