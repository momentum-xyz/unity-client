using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Odyssey;
using Odyssey.Networking;
using System.Text;
using TMPro;

public class PositionUIView : MonoBehaviour, IRequiresContext
{
    public TextMeshProUGUI positionText;
    private StringBuilder sb = new StringBuilder("", 50);
    IMomentumContext _c;
    Vector3 prevPosition = Vector3.zero;

    public void Init(IMomentumContext context)
    {
        _c = context;
    }

    void Update()
    {
        if (_c.Get<ISessionData>().WorldAvatarController == null) return;

        Vector3 currentPosition = _c.Get<ISessionData>().WorldAvatarController.transform.position;

        if (currentPosition != prevPosition)
        {
            OnPositionUpdate(currentPosition);
        }

        prevPosition = currentPosition;
    }


    void OnPositionUpdate(Vector3 pos)
    {
        if (positionText == null) return;

        int textX = (int)(pos.x * 100.0f);
        int textY = (int)(pos.y * 100.0f);
        int textZ = (int)(pos.z * 100.0f);

        sb.Clear();
        sb.Append("<");
        sb.Concat(textX);
        sb.Append(",");
        sb.Concat(textY);
        sb.Append(",");
        sb.Concat(textZ);
        sb.Append(">");

        positionText.SetText(sb);

    }
}

public static class StringBuildExtensions
{
    // These digits are here in a static array to support hex with simple, easily-understandable code. 
    // Since A-Z don't sit next to 0-9 in the ascii table.
    private static readonly char[] ms_digits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

    private static readonly uint ms_default_decimal_places = 5; //< Matches standard .NET formatting dp's
    private static readonly char ms_default_pad_char = '0';

    //! Convert a given unsigned integer value to a string and concatenate onto the stringbuilder. Any base value allowed.
    public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val, uint pad_amount, char pad_char, uint base_val)
    {
        // Calculate length of integer when written out
        uint length = 0;
        uint length_calc = uint_val;

        do
        {
            length_calc /= base_val;
            length++;
        }
        while (length_calc > 0);

        // Pad out space for writing.
        string_builder.Append(pad_char, (int)Mathf.Max(pad_amount, length));

        int strpos = string_builder.Length;

        // We're writing backwards, one character at a time.
        while (length > 0)
        {
            strpos--;

            // Lookup from static char array, to cover hex values too
            string_builder[strpos] = ms_digits[uint_val % base_val];

            uint_val /= base_val;
            length--;
        }

        return string_builder;
    }



    //! Convert a given unsigned integer value to a string and concatenate onto the stringbuilder. Assume no padding and base ten.
    public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val)
    {

        string_builder.Concat(uint_val, 0, ms_default_pad_char, 10);
        return string_builder;
    }

    //! Convert a given unsigned integer value to a string and concatenate onto the stringbuilder. Assume base ten.
    public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val, uint pad_amount)
    {
        string_builder.Concat(uint_val, pad_amount, ms_default_pad_char, 10);
        return string_builder;
    }

    //! Convert a given unsigned integer value to a string and concatenate onto the stringbuilder. Assume base ten.
    public static StringBuilder Concat(this StringBuilder string_builder, uint uint_val, uint pad_amount, char pad_char)
    {
        string_builder.Concat(uint_val, pad_amount, pad_char, 10);
        return string_builder;
    }

    //! Convert a given signed integer value to a string and concatenate onto the stringbuilder. Assume no padding and base ten.
    public static StringBuilder Concat(this StringBuilder string_builder, int int_val)
    {
        string_builder.Concat(int_val, 0, ms_default_pad_char, 10);
        return string_builder;
    }

    //! Convert a given signed integer value to a string and concatenate onto the stringbuilder. Assume base ten.
    public static StringBuilder Concat(this StringBuilder string_builder, int int_val, uint pad_amount)
    {
        string_builder.Concat(int_val, pad_amount, ms_default_pad_char, 10);
        return string_builder;
    }

    //! Convert a given signed integer value to a string and concatenate onto the stringbuilder. Assume base ten.
    public static StringBuilder Concat(this StringBuilder string_builder, int int_val, uint pad_amount, char pad_char)
    {
        string_builder.Concat(int_val, pad_amount, pad_char, 10);
        return string_builder;
    }

    public static StringBuilder Concat(this StringBuilder string_builder, int int_val, uint pad_amount, char pad_char, uint base_val)
    {
        Debug.Assert(pad_amount >= 0);
        Debug.Assert(base_val > 0 && base_val <= 16);

        // Deal with negative numbers
        if (int_val < 0)
        {
            string_builder.Append('-');
            uint uint_val = uint.MaxValue - ((uint)int_val) + 1; //< This is to deal with Int32.MinValue
            string_builder.Concat(uint_val, pad_amount, pad_char, base_val);
        }
        else
        {
            string_builder.Concat((uint)int_val, pad_amount, pad_char, base_val);
        }

        return string_builder;
    }
}

