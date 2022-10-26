bits 16 ; 16-Bit Real Mode
push 0x15F
push 0x22
push 0x23
push string_0


mov si, string_0
call puts
cli
hlt

;
; Prints a string to the screen.
; Params:
;   - ds:si : the string to print
;
puts:
    ; save registers
    push si
    push ax

.loop:
    lodsb 
    or al, al
    jz .done

    mov ah, 0x0e
    mov bh, 0

    int 0x10

    jmp .loop

.done:
    pop ax
    pop si
    ret


; ------- Constants ------
string_0: db 'This is a basic hello world in Davis IR.', 0

