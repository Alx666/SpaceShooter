using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IPoolable
{
    int PoolId { get; }

    Pool<IPoolable> Pool { get; set; }

    void Enable();

    void Disable();
}
