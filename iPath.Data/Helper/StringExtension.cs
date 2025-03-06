using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iPath.Data.Helper;

public static class StringExtension
{
    public static string ShortenTo(this string Text, int Maxlength)
    {
        if( Text.Length < Maxlength  )
        {
            return Text;
        }
        else
        {
            return Text.Substring( 0, Maxlength ) + "...";
        }
    }
}
