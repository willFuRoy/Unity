function function2(...)
	a = select(2, ...)
	print(a)
	print(select(2, ...))
end

function2(4, 5, 6)
