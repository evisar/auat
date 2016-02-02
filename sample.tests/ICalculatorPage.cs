using System;
namespace sample.tests
{
    public interface ICalculatorPage
    {
        void Add(decimal a, decimal b);
        void Substract(decimal a, decimal b);
        void Multiply(decimal a, decimal b);
        void Divide(decimal a, decimal b);        
        decimal Result { get; }
        
    }
}
