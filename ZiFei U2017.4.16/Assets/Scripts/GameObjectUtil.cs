using UnityEngine;

public static  class GameObjectUtil  {



    /// <summary>
    /// 判断对象是否在摄像机内可见
    /// </summary>
    /// <param name="renderer"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

}
