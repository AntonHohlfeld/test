use std::io::{Read, Write};
use std::net::{TcpListener, TcpStream};
use std::thread;
use std::time::{Duration, Instant};

#[derive(Clone, Copy)]
struct TelemetryFrame {
    speed_kph: f32,
    fuel_liters: f32,
    throttle: f32,
    brake: f32,
    position: u8,
    laps_remaining: u8,
}

impl TelemetryFrame {
    fn to_json(&self) -> String {
        format!(
            "{{\"speedKph\":{:.1},\"fuelLiters\":{:.1},\"throttle\":{:.2},\"brake\":{:.2},\"position\":{},\"lapsRemaining\":{}}}",
            self.speed_kph,
            self.fuel_liters,
            self.throttle,
            self.brake,
            self.position,
            self.laps_remaining
        )
    }
}

fn handle_client(mut stream: TcpStream, start: Instant) {
    let mut buf = [0u8; 1024];
    let _ = stream.read(&mut buf);

    let req = String::from_utf8_lossy(&buf);
    let first_line = req.lines().next().unwrap_or("");

    if first_line.contains("GET /health") {
        let body = "ok";
        let response = format!(
            "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {}\r\nAccess-Control-Allow-Origin: *\r\n\r\n{}",
            body.len(), body
        );
        let _ = stream.write_all(response.as_bytes());
        return;
    }

    if first_line.contains("GET /telemetry") {
        let t = start.elapsed().as_secs_f32();
        let throttle = ((t * 1.8).sin() * 0.5 + 0.5).clamp(0.0, 1.0);
        let brake = ((t * 1.4 + 1.2).sin() * 0.5 + 0.5).clamp(0.0, 1.0) * 0.5;
        let speed = (throttle * 300.0 - brake * 120.0).clamp(0.0, 320.0);
        let fuel = (42.0 - t * 0.02).max(0.0);
        let frame = TelemetryFrame {
            speed_kph: speed,
            fuel_liters: fuel,
            throttle,
            brake,
            position: 12,
            laps_remaining: 28,
        };
        let body = frame.to_json();
        let response = format!(
            "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {}\r\nAccess-Control-Allow-Origin: *\r\n\r\n{}",
            body.len(), body
        );
        let _ = stream.write_all(response.as_bytes());
        return;
    }

    let body = "not found";
    let response = format!(
        "HTTP/1.1 404 Not Found\r\nContent-Type: text/plain\r\nContent-Length: {}\r\nAccess-Control-Allow-Origin: *\r\n\r\n{}",
        body.len(), body
    );
    let _ = stream.write_all(response.as_bytes());
}

fn main() {
    let listener = TcpListener::bind("127.0.0.1:32123").expect("bind bridge on 127.0.0.1:32123");
    let start = Instant::now();

    println!("ApexOverlay bridge running: http://127.0.0.1:32123/telemetry");

    for stream in listener.incoming() {
        match stream {
            Ok(stream) => {
                let s = start;
                thread::spawn(move || handle_client(stream, s));
            }
            Err(e) => {
                eprintln!("bridge connection error: {e}");
                thread::sleep(Duration::from_millis(50));
            }
        }
    }
}
