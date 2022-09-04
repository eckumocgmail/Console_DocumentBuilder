 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.DocumentBuilder
{
    public interface IElementFactory<TElement> where TElement : IElement
    {
        TElement Create(string type,
            IDictionary<string, IParameter> properties,
            IDictionary<string, Func<string, IDictionary<string, object>, object>> events);
    }



    /// <summary>
    /// ///
    /// </summary>
    public interface IElement
    {
        public string ID { get; set; }
        public string Name { get; set; }

    }



    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    public abstract class AbstractElement<TParameter> : IElement where TParameter : IParameter
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IContainer<AbstractElement<TParameter>> Parent { get; set; }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValidation"></typeparam>
    public class XsdElement<TValidation> : AbstractElement<ValidationParameter>, IContainer<IValidation> where TValidation : IValidation
    {
        public TValidation Validator { get; set; }

        public IValidation Append(IValidation element)
        {
            if( element.Validate(this.Name))
            {
                return element;
            }
            return null;
        }

        public IValidation Replace(IValidation element)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class Authentication: ValidationParameter, IValidation
    {

        private string _name;
        public string GetName() => _name;
        public string SetName(string name) => (_name= name);


        public bool Validate(object value) => (new Random().NextDouble() < 0.5f);


        public string GetSuccessMessage() => $"Hello {GetName()}";

        public string GetFailedMessage() => $"No valid {GetName()}";
    }


    /// <summary>
    /// 
    /// </summary>
    public class ComplexType: XrdsContainer<Authentication>
    {
        public override void OnAppend(IContainer<XsdElement<Authentication>> container, XsdElement<Authentication> element)
        {
            throw new NotImplementedException();
        }

        public override void OnReplace(IContainer<XsdElement<Authentication>> container, XsdElement<Authentication> element)
        {
            throw new NotImplementedException();
        }
    }


    public interface IContainer<TElement>
    {
        public TElement Append(TElement element);
        public TElement Replace(TElement element);
    }

    

    public abstract class AbstractContainer<TElement> : IContainer<TElement> where TElement : AbstractElement<ValidationParameter>
    {
        private List<TElement> elements = new List<TElement>();
        public abstract void OnAppend(IContainer<TElement> container, TElement element);
        public abstract void OnReplace(IContainer<TElement> container, TElement element);
        public TElement Append(TElement element)
        {
            if (element.Parent != null)
            {
                element.Parent.Replace(element);
            }
            elements.Add(element);
            OnAppend(this,element);
            return element;
        }

        public TElement Replace(TElement element)
        {

            elements.Remove(element);
            OnReplace(this, element);
            return element;
        }
    }


    public abstract class XrdsContainer<TValidation> : AbstractContainer<XsdElement<TValidation>> where TValidation : IValidation

    {
        protected IElementFactory<XsdElement<TValidation>> factory { get; set; }

        public abstract override void OnAppend(IContainer<XsdElement<TValidation>> container, XsdElement<TValidation> element);
        public abstract override void OnReplace(IContainer<XsdElement<TValidation>> container, XsdElement<TValidation> element);
        
    }



    public class HeaderValidation: IValidation
    {
        public string GetName()
        {
            throw new NotImplementedException();
        }

        public string SetName(string name)
        {
            throw new NotImplementedException();
        }

        public bool Validate(object value)
        {
            throw new NotImplementedException();
        }

        public string GetSuccessMessage()
        {
            throw new NotImplementedException();
        }

        public string GetFailedMessage()
        {
            throw new NotImplementedException();
        }
    }
    public class DiscoveryService: XsdElement<HeaderValidation>  
    {
    }

  
    public class XrdsDocument : XrdsContainer<Authentication>
    {
         

        public Encoding Encoding => Encoding.ASCII;

        public XrdsDocument() : base()
        {

        }



        public override void OnAppend(IContainer<XsdElement<Authentication>> container, XsdElement<Authentication> element) {
            var updated = 
            this.Append(this.factory.Create("SomeAuth",
                new Dictionary<string, IParameter>(),
                new Dictionary<string, Func<string, IDictionary<string, object>, object>>()));          
        }
             

        public override void OnReplace(IContainer<XsdElement<Authentication>> container, XsdElement<Authentication> element)
        {
            throw new NotImplementedException();
        }
    }























    
}
