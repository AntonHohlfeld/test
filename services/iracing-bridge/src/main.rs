use serde::Serialize;

#[derive(Serialize)]
#[serde(rename_all = "camelCase")]
struct TelemetryFrame {
    speed_kph: f32,
    fuel_liters: f32,
    throttle: f32,
    brake: f32,
}

fn main() {
    let frame = TelemetryFrame {
        speed_kph: 123.4,
        fuel_liters: 42.0,
        throttle: 0.81,
        brake: 0.0,
    };

    println!("{}", serde_json::to_string(&frame).expect("serialize telemetry frame"));
}
