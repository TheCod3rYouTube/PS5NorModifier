var serialPort;

function serialIsSupported() {
    return navigator.serial ? true : false;
}

async function serialRequestPort() {
    try {
        serialPort = await navigator.serial.requestPort();
        return true;
    }
    catch (ex) {
        console.error("Serial request error:", err);
        return false;
    }
}

async function serialOpen(baudRate) {
    try {
        await serialPort.open({ baudRate: baudRate });
        //await serialPort.setSignals({ rts: true });
        return true;
    }
    catch (ex) {
        console.error("Serial open error:", err);
        return false;
    }
}

async function serialClose() {
    await serialPort.close();
}

async function serialWrite(text) {
    let writer;
    let writableStreamClosed;
    try {
        const textEncoder = new TextEncoderStream();
        writableStreamClosed = textEncoder.readable.pipeTo(serialPort.writable);
        writer = textEncoder.writable.getWriter();

        await writer.write(text);
        return true;
    } catch (err) {
        console.error("Serial write error:", err);
    } finally {
        if (writer) {
            await writer.close();
            await writableStreamClosed;
            writer.releaseLock();
        }
    }

    return false;
}

async function serialRead() {
    const textDecoder = new TextDecoderStream();
    const readableStreamClosed = serialPort.readable.pipeTo(textDecoder.writable);
    const reader = textDecoder.readable.getReader();

    let buffer = '';

    try {
        while (true) {
            const { value, done } = await reader.read();
            if (done) break;

            buffer += value;

            const match = buffer.match(/(.*?)(\r\n|\n|\r)/);
            if (match) {
                return match[1].trim();
            }
        }
    } catch (err) {
        console.error("Serial read error:", err);
    } finally {
        await reader.cancel();
        reader.releaseLock();
        try { await readableStreamClosed; } catch (_) { }
    }

    return null;
}

