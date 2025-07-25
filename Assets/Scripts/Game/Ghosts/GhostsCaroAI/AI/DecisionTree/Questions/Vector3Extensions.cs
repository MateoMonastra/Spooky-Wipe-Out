using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 IgnoreY(this Vector3 self)
    {
        var result = new Vector3(self.x, 0, self.z);
        return result;
    }
}