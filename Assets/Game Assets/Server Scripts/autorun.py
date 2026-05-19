import server_test, time



while True:
    server_test.init()
    server_test.post_score(1, 'john test')
    time.sleep(60 * 5)