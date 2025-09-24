## 放置引擎-配方
extends Resource
class_name IdleFramework_Recipe

## ID，该配方的唯一标识ID
## 此项留空将使该配方在注册时失效
@export var id: StringName

## 原材料表
## 如果某一原料的需求数量是会变化的，生产器会在满足该配方订单被启动时的需求数量时计时生产，在计时完成后扣除的原料数量将再次从数值提供器获取，如果原料数量不满足此时的需要，该次生产将失败并使生产计时重置到0%
@export var ingredients: Array[IdleFramework_RecipeItem]

## 配方制作所需时间，单位为秒
## 原材料满足时计时，计时满后扣除原材料并产出产品
@export var required_seconds: IdleFramework_NumberProvider

## 产品表
@export var result: Array[IdleFramework_RecipeItem]
