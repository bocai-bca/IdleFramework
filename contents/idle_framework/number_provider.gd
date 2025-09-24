## 放置引擎-数值提供器，可配置地以多种模式中的其一提供整型数字
## 通过选择该类的任意一种子类来选择模式
@abstract
extends Resource
class_name IdleFramework_NumberProvider

## 获取数字
## 子类必须实现的抽象方法，需以子类特有的模式返回数字
@abstract func _get_number() -> int
