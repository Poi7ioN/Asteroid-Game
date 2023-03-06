using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Define an interface for objects that can be damaged
public interface IDamagable
{
    // Declare a method that takes an integer parameter and call on damage taken.
    void Damage(int dmg);
}