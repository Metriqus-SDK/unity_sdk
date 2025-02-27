using System.Collections.Generic;

namespace MetriqusSdk
{
    /// <summary>
    /// Represents a custom event with a unique key and a list of associated parameters.
    /// </summary>
    public class MetriqusCustomEvent
    {
        /// <summary>
        /// The unique identifier for the event.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// A list of parameters associated with the event.
        /// </summary>
        protected List<TypedParameter> Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusCustomEvent"/> class with a specified event key.
        /// </summary>
        /// <param name="key">The unique identifier for the event.</param>
        public MetriqusCustomEvent(string key)
        {
            this.Key = key;
            this.Parameters = new();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetriqusCustomEvent"/> class with a specified event key and parameters.
        /// </summary>
        /// <param name="key">The unique identifier for the event.</param>
        /// <param name="parameters">A list of parameters associated with the event.</param>
        public MetriqusCustomEvent(string key, List<TypedParameter> parameters)
        {
            this.Key = key;
            this.Parameters = parameters ?? null;
        }

        /// <summary>
        /// Adds a parameter to the event.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        public void AddParameter(TypedParameter parameter)
        {
            if (this.Parameters == null)
            {
                this.Parameters = new List<TypedParameter>();
            }

            this.Parameters.Add(parameter);
        }

        public virtual List<TypedParameter> GetParameters() { return this.Parameters; }
    }
}