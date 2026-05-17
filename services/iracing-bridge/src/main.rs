struct TelemetryFrame {
    speed_kph: f32,
    fuel_liters: f32,
    throttle: f32,
    brake: f32,
}

impl TelemetryFrame {
    fn to_json(&self) -> String {
        format!(
            "{{\"speedKph\":{:.1},\"fuelLiters\":{:.1},\"throttle\":{:.2},\"brake\":{:.1}}}",
            self.speed_kph, self.fuel_liters, self.throttle, self.brake
        )
    }
}

fn main() {
    let frame = TelemetryFrame {
        speed_kph: 123.4,
        fuel_liters: 42.0,
        throttle: 0.81,
        brake: 0.0,
    };

    println!("{}", frame.to_json());
}
