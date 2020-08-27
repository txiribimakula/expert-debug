﻿using Txiribimakula.ExpertWatch.Drawing;

namespace Txiribimakula.ExpertWatch.Loading
{
    public interface IInterpreter
    {
        IDrawable GetDrawable(ExpressionLoader expression);
    }
}
