## 放置引擎-配方物品，用于配方中的物品，除物品ID外还可配置数量
extends Resource
class_name IdleFramework_RecipeItem

## ID，该物品的唯一标识ID
## 需要与物品注册表中的ID对应
@export var id: StringName

## 数字提供器，指代该配方物品的所需数量或产出数量
@export var number_provider: IdleFramework_NumberProvider
