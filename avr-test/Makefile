
main.bin: main.elf
	avr-objcopy -j .text -j .data -O binary $^ $@

main.elf: main.o
	avr-gcc $(CFLAGS) $< -o $@

.c.o:
	avr-gcc $(CFLAGS) -c $< -o $@

clean:
	del /s main.bin main.elf main.o

.PHONY: clean