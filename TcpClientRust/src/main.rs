use std::env;
use std::error::Error;
use std::io::{Read, Write};
use std::net::TcpStream;

fn run() -> Result<(), Box<dyn Error>> {
    let server_ip = env::args()
        .nth(1)
        .unwrap_or_else(|| "127.0.0.1".to_string());
    let addr = format!("{}:5000", server_ip);
    let mut stream = TcpStream::connect(&addr)?;
    stream.set_nodelay(true)?;
    println!("Connected to server.");

    let mut receive_buffer = [0u8; 512];
    let mut total_received = 0;
    while total_received < 512 * 1000 {
        let bytes_read = stream.read(&mut receive_buffer)?;
        if bytes_read == 0 {
            return Ok(());
        }
        total_received += bytes_read;
    }

    stream.write_all(&[1])?;
    stream.flush()?;

    let mut result_buffer = [0u8; 64];
    let len = stream.read(&mut result_buffer)?;
    let result = String::from_utf8_lossy(&result_buffer[..len]);
    println!("Elapsed: {} ms", result);

    Ok(())
}

fn main() {
    if let Err(e) = run() {
        println!("Exception: {}", e);
    }
    println!("Disconnected.");
}
