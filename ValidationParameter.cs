using Patterns.DocumentBuilder;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Patterns.DocumentBuilder
{

    public interface IParameter
    {
        public T Get<T>(string name);
        public T Set<T>(string name, T value);
    }
    public interface IValidation
    {
        public string GetName();
        public string SetName(string name);
        public bool Validate(object value);
        public string GetSuccessMessage();
        public string GetFailedMessage();
    }
    public abstract class AbstractValidation<TValidation> : IValidation, IContainer<TValidation>
    {
        public abstract string GetName();
        public abstract string SetName(string name);
        public abstract bool Validate(object context, object value);
        public abstract TValidation Append(TValidation element);
        public abstract TValidation Replace(TValidation element);
        public abstract bool Validate(object value);
        public abstract string GetSuccessMessage();
        public abstract string GetFailedMessage();
    }
    public abstract class AbstractParameter<TValidation> : IParameter where TValidation : IValidation
    {
        private object _value { get; set; }
        protected abstract IDictionary<string, IDictionary<string, object>> Validate();
        public T Get<T>(string name) => (T)_value;
        public T Set<T>(string name, T value) => (T)(_value = value);
        public Func<object, object, bool> OnChanges = (prev, current) => {
            return true;
        };
    }
    public class ValidationParameter : AbstractParameter<AbstractValidation<IValidation>>, IContainer<IValidation> 
    {
        private IList<IValidation> Validations { get; set; } = new List<IValidation>();
        public IDictionary<string, IValidation> AsDictionary() =>
            new Dictionary<string, IValidation>(this.Validations.ToList().Select(val => new KeyValuePair<string, IValidation>(val.GetName(), val)));

        public IValidation Append(IValidation element)
        {
            Validations.Add(element);
            return element;
        }

        public IValidation Replace(IValidation element)
        {
            Validations.Remove(element);
            return element;
        }

        protected override IDictionary<string, IDictionary<string, object>> Validate()
        {
            IDictionary<string, bool> results = new Dictionary<string, bool>();
            IDictionary<string, string> messages = new Dictionary<string, string>();
            object value = this.Get<object>("self");
            foreach (var validation in Validations)
            {
                if ((results[validation.GetName()] = results.ContainsKey(validation.GetName())))
                {
                    throw new Exception("Validation results can be destroyed.");
                }
                if (validation.Validate(value))
                {
                    messages[validation.GetName()] = validation.GetSuccessMessage();
                }
            }
            var boxedResults = new Dictionary<string, object>(results.Select(kv => new KeyValuePair<string, object>(kv.Key, (object)kv.Value)));
            var boxedMessages = new Dictionary<string, object>(messages.Select(kv => new KeyValuePair<string, object>(kv.Key, (object)kv.Value)));
            return new Dictionary<string, IDictionary<string, object>>()
            {
                { "results", boxedResults },
                { "messages", boxedMessages },
            };
        }



        public class RusWord : Attribute, IValidation  
        {
            public string GetName() => "";
            public string SetName(string name) => "";            
            public string GetSuccessMessage() => "";
            public string GetFailedMessage() => "";
            public bool Validate(object value) => false;
        }

        /// глагол
        public class RusNoun : RusWord { 

        }
        /// глагол
        public class RusAdjective : RusWord { }
        

        public static void Run(string[] args)
        {
            var parameter = new RusWord();
            parameter.Append(new RusWord());
        }
    }
}