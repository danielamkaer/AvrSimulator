
main.bin: main.elf
	avr-objcopy -j .text -j .data -O binary $^ $@

main.elf: main.o
	avr-gcc -O3 -g3 $(CFLAGS) $< -o $@

.c.o:
	avr-gcc -O3 -g3 $(CFLAGS) -c $< -o $@

clean:
	del /s main.bin main.elf main.o

.PHONY: clean
