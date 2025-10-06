## 放置引擎-容器实例
extends RefCounted
class_name IdleFramework_ContainerInstance

## 物品最大数量覆写
## 键为物品ID，值为该物品可以存储的最大数量
var item_max_count_override: Dictionary[StringName, IdleFramework_NumberProvider]

## 存储的物品
## 键为物品ID，值为该物品的数量
var items: Dictionary[StringName, int]
