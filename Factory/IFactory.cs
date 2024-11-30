using System;

namespace GuestUnion.Factory {

    public interface IFactory<out TResult> {

        TResult Create();
    }
}