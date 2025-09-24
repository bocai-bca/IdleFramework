## 放置引擎-数值提供器-立即数
extends IdleFramework_NumberProvider
class_name IdleFramework_NumberProvider_Direct

## 数字，将固定提供该数字
@export var number: int

## 获取数字
func _get_number() -> int:
	return number
