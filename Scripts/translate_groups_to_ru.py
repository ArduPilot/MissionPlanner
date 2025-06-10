import sys
import xml.etree.ElementTree as ET

translations = {
    "UAVCAN Motor Parameters": "Параметры моторов UAVCAN",
    "Attitude EKF estimator": "Оцениватель положения EKF",
    "Attitude Q estimator": "Оцениватель положения Q",
    "Battery Calibration": "Калибровка аккумулятора",
    "Camera trigger": "Запуск камеры",
    "Circuit Breaker": "Предохранители",
    "Commander": "Командный модуль",
    "Data Link Loss": "Потеря связи",
    "FW Attitude Control": "Управление ориентацией самолёта",
    "Fixed Wing TECS": "TECS самолёта",
    "GPS Failure Navigation": "Навигация при отказе GPS",
    "Geofence": "Виртуальное ограждение",
    "Gimbal": "Подвес",
    "L1 Control": "Управление L1",
    "Land Detector": "Детектор посадки",
    "Launch detection": "Обнаружение запуска",
    "Local Position Estimator": "Локальный оцениватель позиции",
    "MAVLink": "MAVLink",
    "MKBLCTRL Testmode": "Тестовый режим MKBLCTRL",
    "Multicopter Attitude Control": "Управление ориентацией мультикоптера",
    "Multicopter Position Control": "Управление положением мультикоптера",
    "PWM Outputs": "ШИМ выходы",
    "Payload drop": "Сброс нагрузки",
    "Position Estimator": "Оцениватель позиции",
    "Position Estimator INAV": "Оцениватель позиции INAV",
    "Radio Calibration": "Калибровка радиосвязи",
    "Radio Signal Loss": "Потеря радиосигнала",
    "Radio Switches": "Радиовыключатели",
    "Return To Land": "Возврат и посадка",
    "Sensor Calibration": "Калибровка датчиков",
    "Sensor Enable": "Включение датчиков",
    "Subscriber Example": "Пример подписчика",
    "System": "Система",
    "UAVCAN": "UAVCAN",
    "VTOL Attitude Control": "Управление ориентацией VTOL",
    "mTECS": "mTECS",
    "Miscellaneous": "Разное"
}

if __name__ == '__main__':
    if len(sys.argv) != 3:
        print('Usage: python translate_groups_to_ru.py <input_xml> <output_xml>')
        sys.exit(1)
    tree = ET.parse(sys.argv[1])
    root = tree.getroot()
    for grp in root.findall('group'):
        name = grp.attrib.get('name')
        if name in translations:
            grp.attrib['name'] = translations[name]
    tree.write(sys.argv[2], encoding='utf-8', xml_declaration=True)
