#define TEST (*(unsigned int *)(0x100))
#define OUT (*(volatile unsigned char *)(0x20))

void main(void)
{
	const char *str = "Hello World";
	while (*str != 0) {
		OUT = *str++;
	}
}