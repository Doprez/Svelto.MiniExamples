﻿namespace Svelto.ECS.Example.Survive.Damage
{
    public class isDying : GroupTag<Dead> { };
    public class Dead : GroupTag<Dead> { };

    public class Damageable : GroupTag<Damageable> { };
}