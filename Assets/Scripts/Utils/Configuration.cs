using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Configuration
{
    public static readonly LayerMask GroundLayer =  LayerMask.GetMask("Ground");
    public const int ThrowablesLayer = 14;

    public static class Input
    {
        public const string JumpButton = "Jump";
        public const string GrappleButton = "Grapple";
        public const string FlyButton = "Fly";
        public const string DashButton = "Dash";
        public const string WallClimbButton = "WallClimb";
        public const string HorizontalAxis = "Horizontal";
        public const string VerticalAxis = "Vertical";
        public const string SwapButton = "Swap";
        public const string CancelButton= "Cancel";
        public const string ActionButton = "Action";
        public const string Interact = "Interact";
    }

    public static class AnimatorParameters
    {
        public const string DashTrigger = "dash";
        public const string JumpTrigger = "jump";
    }


    public static class Tags
    {
        public const string JumpTrigger = "JumpTrigger";
        public const string SwapField = "SwapField";
        public const string SwapTarget = "SwapTarget";
        public const string GrapplePoint = "GrapplePoint";
        public const string ThrowableObject = "ThrowableObject";
    }
}
