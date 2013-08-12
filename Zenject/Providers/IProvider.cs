using System;
namespace Zenject
{
    public interface IProvider
    {
        object GetInstance();
        Type GetInstanceType();
    }
}
