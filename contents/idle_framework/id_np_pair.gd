## 放置引擎-ID数字提供器对
extends Resource
class_name IdleFramework_IdNpPair

## ID。
## 用于配方的原材料或产物时，指代该物品的ID。
## 用于容器的物品最大数量时，指代该物品的ID
@export var id: StringName

## 数值提供器。
## 用于配方的原材料或产物时，指代该配方物品的所需数量或产出数量。
## 用于容器的物品最大数量时，指代该容器可存储该物品的最大数量
@export var number_provider: IdleFramework_NumberProvider

## 设置ID，用于通过代码创建实例时链式调用
func id_(new_id: StringName) -> IdleFramework_IdNpPair:
	id = new_id
	return self

## 设置数值提供器，用于通过代码创建实例时链式调用
func number_provider_(new_num_provider: IdleFramework_NumberProvider) -> IdleFramework_IdNpPair:
	number_provider = new_num_provider
	return self
