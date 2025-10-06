## 放置引擎-配方
extends Resource
class_name IdleFramework_Recipe

## 原材料表。
## 如果原材料配方物品的数值提供器返回结果是可变的，生产器会在配方订单创建时确定需求数量，在满足该需求数量的情况下才进行生产计时，在计时完毕后再次访问数值提供器取得需求数量，并扣除此时得到的需求数量个原料，而如果原料库存量未能满足此时的需求数量，该次生产将被重置到初始状态。
## 如果需要的物品数量小于等于0，则需要在库存至少存有1个该物品时才算可生产，但生产完毕时不消耗该物品。
## 键为物品ID，值为需要的物品数量
@export var ingredients: Dictionary[StringName, IdleFramework_NumberProvider]

## 配方制作所需时间，单位为秒。
## 原材料满足时计时，计时满后扣除原材料并产出产品。
## 如果给定的数值提供器的返回结果是可变的，此次配方订单生产所需时间会在开始生产时从数值提供器获取，该时间会缓存在该配方订单数据中
@export var required_seconds: IdleFramework_NumberProvider

## 产品表。
## 如果产品配方物品的数值提供器返回的结果是可变的，没啥大碍，可变数值提供器就是为这种位置量身打造的。
## 键为物品ID，值为需要的物品数量
@export var result: Dictionary[StringName, IdleFramework_NumberProvider]
