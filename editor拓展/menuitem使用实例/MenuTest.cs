/*********************************************************************************
 *Copyright(C) 2015 by FengNongtheWindCoder
 *All rights reserved.
 *FileName:     MenuTest.cs
 *Author:       FengNongtheWindCoder
 *Version:      1.0
 *UnityVersion：5.3.0f4
 *Date:         12/25/2015 10:27:05 PM
 *Description:  对menuitem各种用法的展示
                如有问题请在 https://git.coding.net/better-start-now/unityCodeBase.git 讨论区留言。
 *History:  
**********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Collections;

public class MenuTest : MonoBehaviour {

    // 在顶部菜单增加一个MyMenu菜单，其内有Do Something菜单项
    [MenuItem("MyMenu/Do Something")]
    static void DoSomething()
    {
        Debug.Log("Doing Something...");
    }


    // 对菜单项的验证Validated menu item.
    // 添加一个Log Selected Transform Name到MyMenu里
    // 需要使用另外一个函数来进行验证
    // 确保只在选中transform时使能
    [MenuItem("MyMenu/Log Selected Transform Name")]
    static void LogSelectedTransformName()
    {
        Debug.Log("Selected Transform is on " + Selection.activeTransform.gameObject.name + ".");
    }

    // 验证上面的菜单项
    // 此函数返回false时，上面的菜单项不使能
    [MenuItem("MyMenu/Log Selected Transform Name", true)]
    static bool ValidateLogSelectedTransformName()
    {
        // 没有选中transform返回false
        return Selection.activeTransform != null;
    }


    // 增加一个菜单项名为"Do Something with a Shortcut Key" 到 MyMenu里
    // 并添加快捷键 (ctrl-g on Windows, cmd-g on OS X).
    [MenuItem("MyMenu/Do Something with a Shortcut Key %g")]
    static void DoSomethingWithAShortcutKey()
    {
        Debug.Log("Doing something with a Shortcut Key...");
    }

    // 添加一个 "Double Mass"菜单项到 Rigidbody 的右键菜单
    [MenuItem("CONTEXT/Rigidbody/Double Mass")]
    static void DoubleMass(MenuCommand command)
    {
        Rigidbody body = (Rigidbody)command.context;
        body.mass = body.mass * 2;
        Debug.Log("Doubled Rigidbody's Mass to " + body.mass + " from Context Menu.");
    }

    // 添加创建自定义GameObjects的菜单项
    // 优先级10代表它会跟其他创建菜单项组在一起
    // 并使其可以在层次视图的create下拉菜单和右键菜单中出现
    [MenuItem("GameObject/MyCategory/Custom Game Object", false, 10)]
    static void CreateCustomGameObject(MenuCommand menuCommand)
    {
        // 创建一个自定义对象
        GameObject go = new GameObject("Custom Game Object");
        // 确保对选中对象通过右键菜单创建时，重设新对象的父亲为当前选中对象，没有的话什么都不会执行
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // 确保当前创建操作注册到撤销系统里，使其可以正常撤销
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        //选中新创建的对象
        Selection.activeObject = go;
    }
}
