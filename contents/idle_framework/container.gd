## 放置引擎-容器，用于存储物品
## 容器注册是唯一的，需通过ID来访问注册对象
extends Resource
class_name IdleFramework_Container

## 物品最大数量覆写
## 键为物品ID，值为该物品可存储的最大数量
@export var item_max_count_override: Dictionary[StringName, IdleFramework_NumberProvider]

## 设置物品最大数量覆写，用于通过代码创建实例时链式调用
func item_max_count_override_(override: Dictionary[StringName, IdleFramework_NumberProvider]) -> IdleFramework_Container:
	item_max_count_override = override
	return self

func make_instance() -> IdleFramework_ContainerInstance:
	var new_instance: IdleFramework_ContainerInstance = IdleFramework_ContainerInstance.new()
	new_instance.item_max_count_override = item_max_count_override
	return new_instance
