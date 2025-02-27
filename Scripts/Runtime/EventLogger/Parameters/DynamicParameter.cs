using System.Collections.Generic;

namespace MetriqusSdk
{
    internal class DynamicParameter : IParameter
    {
        private string name;
        private object value;

        public string Name => name;
        public object Value => value;

        public DynamicParameter(string parameterName, string parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, long parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, double parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, int parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, bool parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, object parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, IDictionary<string, object> parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }

        public DynamicParameter(string parameterName, IEnumerable<IDictionary<string, object>> parameterValue)
        {
            name = parameterName;
            value = parameterValue;
        }
    }
}