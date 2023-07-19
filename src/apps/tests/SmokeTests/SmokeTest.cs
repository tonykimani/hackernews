using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tests.Contracts;

namespace tests.SmokeTests
{
    public class SmokeTest : ISmokeTest
    {
        public Task Start()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Log.Error($"Failed while smoke testing.{ex.Message}.{ex.StackTrace}", ex);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
