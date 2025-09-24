## 放置引擎-数值提供器-范围随机数
extends IdleFramework_NumberProvider
class_name IdleFramework_NumberProvider_Range

## 最小值(含)
@export var min_number: int

## 最大值(含)
@export var max_number: int

## 获取数字
func _get_number() -> int:
	return randi_range(min_number, max_number)
