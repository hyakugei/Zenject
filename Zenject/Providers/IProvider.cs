using System;
namespace ModestTree.Zenject
{
    public interface IProvider
    {
        object GetInstance();
        Type GetInstanceType();
    }
}
