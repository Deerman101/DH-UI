using UnityEngine;
using System.Collections;
using System;

public class ObservableProperty<T> // Да-да, можно было просто использовать UniRX, но так интереснее ;)
{
    private T value;

    public event Action<T> OnChanged;

    public T Value
    {
        get => value;
        set
        {
            this.value = value;
            OnChanged?.Invoke(value);
        }
    }
}
